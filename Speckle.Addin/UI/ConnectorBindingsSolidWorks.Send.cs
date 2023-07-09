using DesktopUI2;
using DesktopUI2.Models;
using DesktopUI2.Models.Filters;
using DesktopUI2.ViewModels;
using Serilog.Context;
using SolidWorks.Interop.sldworks;
using Speckle.ConnectorSolidWorks.Selection;
using Speckle.ConnectorSolidWorks.Utils;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Speckle.ConnectorSolidWorks.UI;

public partial class ConnectorBindingsSolidWorks
{
    public override void PreviewSend(StreamState state, ProgressViewModel progress)
    {
        try
        {
            var filterObjs = GetSelectionFilterObjects(state.Filter);
            foreach (var filterObj in filterObjs)
            {
                var converter = (ISpeckleConverter)Activator.CreateInstance(Converter.GetType());
                var reportObj = new ApplicationObject(filterObj.PID, filterObj.SelectType.ToString());
                if (!converter.CanConvertToSpeckle(filterObj))
                    reportObj.Update(
                      status: ApplicationObject.State.Skipped,
                      logItem: $"Sending this object type is not supported in SolidWorks");
                else
                    reportObj.Update(status: ApplicationObject.State.Created);
                progress.Report.Log(reportObj);
            }

            SelectClientObjects(filterObjs.Select(o => o.PID).ToList(), true);
        }
        catch (Exception ex)
        {
            SpeckleLog.Logger.Error(
              ex,
              "Failed to preview send: {exceptionMessage}",
              ex.Message
            );
        }
    }

    public async override Task<string> SendStream(
        StreamState state,
        ProgressViewModel progress)
    {
        IModelDoc2 doc = App.IActiveDoc2;

        //make sure to instance a new copy so all values are reset correctly
        var converter = (ISpeckleConverter)Activator.CreateInstance(Converter.GetType());
        converter.SetContextDocument(doc);
        converter.Report.ReportObjects.Clear();

        // set converter settings as tuples (setting slug, setting selection)
        var settings = new Dictionary<string, string>();
        CurrentSettings = state.Settings;
        foreach (var setting in state.Settings)
            settings.Add(setting.Slug, setting.Selection);
        converter.SetConverterSettings(settings);

        var streamId = state.StreamId;
        var client = state.Client;

        List<SwSeleTypeObjectPair> selectedObjects = GetSelectionFilterObjects(state.Filter).ToList();
        state.SelectedObjectIds = selectedObjects.Select(x => x.PID).ToList();

        if (!selectedObjects.Any())
            throw new InvalidOperationException(
              "There are zero objects to send. Please use a filter, or set some via selection."
            );

        converter.SetContextObjects(
          selectedObjects
            .Select(x => new ApplicationObject(x.PID, x.GetType().ToString()) { applicationId = x.PID })
            .ToList()
        );
        var commitObject = converter.ConvertToSpeckle(doc) ?? new Collection();
        SolidWorksCommitObjectBuilder commitObjectBuilder = new();

        progress.Report = new ProgressReport();
        progress.Max = selectedObjects.Count();

        var conversionProgressDict = new ConcurrentDictionary<string, int> { ["Conversion"] = 0 };
        var convertedCount = 0;

        using var _d0 = LogContext.PushProperty("conversionDirection", nameof(ISpeckleConverter.ConvertToSpeckle));

        foreach (var swObject in selectedObjects)
        {
            if (progress.CancellationToken.IsCancellationRequested)
                break;

            bool isAlreadyConverted = GetOrCreateApplicationObject(
                swObject,
                converter.Report,
                out ApplicationObject reportObj
              );
            if (isAlreadyConverted)
                continue;

            progress.Report.Log(reportObj);

            //Add context to logger
            using var _d1 = LogContext.PushProperty("SwObjectName", swObject.Name);
            using var _d2 = LogContext.PushProperty("SwSelectType", swObject.SelectType);

            try
            {
                converter.Report.Log(reportObj); // Log object so converter can access

                Base result = ConvertToSpeckle(swObject, converter);

                reportObj.Update(
                status: ApplicationObject.State.Created,
                logItem: $"Sent as {SimplifySpeckleType(result.speckle_type)}"
              );
                if (result.applicationId != reportObj.applicationId)
                {
                    SpeckleLog.Logger.Information(
                    "Conversion result of type {elementType} has a different application Id ({actualId}) to the report object {expectedId}",
                    swObject.GetType(),
                    result.applicationId,
                    reportObj.applicationId
                  );
                    result.applicationId = reportObj.applicationId;
                }
                commitObjectBuilder.IncludeObject(result, swObject);
                convertedCount++;
            }
            catch (ConversionSkippedException ex)
            {
                reportObj.Update(status: ApplicationObject.State.Skipped, logItem: ex.Message);
            }
            catch (Exception ex)
            {
                SpeckleLog.Logger.Error(ex, "Object failed during conversion");
                reportObj.Update(status: ApplicationObject.State.Failed, logItem: $"{ex.Message}");
            }

            conversionProgressDict["Conversion"]++;
            progress.Update(conversionProgressDict);
        }

        progress.Report.Merge(converter.Report);

        progress.CancellationToken.ThrowIfCancellationRequested();

        if (convertedCount == 0)
        {
            throw new SpeckleException("Zero objects converted successfully. Send stopped.");
        }

        commitObjectBuilder.BuildCommitObject(commitObject);

        var transports = new List<ITransport>() { new ServerTransport(client.Account, streamId) };

        var objectId = await Operations
          .Send(
            @object: commitObject,
            cancellationToken: progress.CancellationToken,
            transports: transports,
            onProgressAction: dict => progress.Update(dict),
            onErrorAction: ConnectorHelpers.DefaultSendErrorHandler,
            disposeTransports: true
          )
          .ConfigureAwait(true);

        progress.CancellationToken.ThrowIfCancellationRequested();

        var actualCommit = new CommitCreateInput()
        {
            streamId = streamId,
            objectId = objectId,
            branchName = state.BranchName,
            message = state.CommitMessage ?? $"Sent {convertedCount} objects from {APP_NAME}.",
            sourceApplication = APP_NAME,
        };

        if (state.PreviousCommitId != null)
        {
            actualCommit.parents = new List<string>() { state.PreviousCommitId };
        }

        var commitId = await ConnectorHelpers
          .CreateCommit(client, actualCommit, progress.CancellationToken)
          .ConfigureAwait(false);

        return commitId;
    }

    #region Private Methods
    private Base ConvertToSpeckle(
        SwSeleTypeObjectPair swObject, 
        ISpeckleConverter converter)
    {
        if (!converter.CanConvertToSpeckle(swObject))
        {
            string skipMessage = "Sending this object type is not supported yet";

            throw new ConversionSkippedException(skipMessage, swObject);
        }

        Base conversionResult = converter.ConvertToSpeckle(swObject);

        if (conversionResult == null)
            throw new SpeckleException($"Conversion of {swObject.PID} (ToSpeckle) returned null");

        return conversionResult;
    }

    private bool GetOrCreateApplicationObject(
        SwSeleTypeObjectPair swObject, 
        ProgressReport report, 
        out ApplicationObject reportObj)
    {
        if (report.ReportObjects.TryGetValue(swObject.PID, out var applicationObject))
        {
            reportObj = applicationObject;
            return true;
        }

        reportObj = new(swObject.PID, swObject.SelectType.ToString()) { applicationId = swObject.PID };
        return false;
    }

    /// <summary>
    /// Removes all inherited classes from speckle type string
    /// </summary>
    private static string SimplifySpeckleType(string type)
    {
        return type.Split(
            new char[] { ':' }, 
            StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
    }

    /// <summary>
    /// Get selection by filter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private IEnumerable<SwSeleTypeObjectPair> GetSelectionFilterObjects(ISelectionFilter filter)
    {
        var doc = App.IActiveDoc2;
        return SwDocFilterUtils.Filter(doc, filter);
    }
    #endregion
}
