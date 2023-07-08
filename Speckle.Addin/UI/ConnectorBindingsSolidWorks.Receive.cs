using DesktopUI2;
using DesktopUI2.Models;
using DesktopUI2.Models.Settings;
using DesktopUI2.ViewModels;
using SolidWorks.Interop.sldworks;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Models.GraphTraversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Speckle.ConnectorSolidWorks.UI;

public partial class ConnectorBindingsSolidWorks
{
    public List<ISetting>? CurrentSettings { get; private set; }

    private string? SelectedReceiveCommit { get; set; }

    public Dictionary<string, Base> StoredObjects { get; private set; } = new Dictionary<string, Base>();

    public List<ApplicationObject> Preview { get; private set; } = new List<ApplicationObject>();

    /// <summary>
    /// Converter for SolidWorks connector.
    /// </summary>
    /// <remarks>
    /// Only use an instance of the converter as a local variable to avoid conflicts if multiple sending/receiving
    /// </remarks>
    public ISpeckleConverter Converter { get; private set; } = KitManager.GetDefaultKit().LoadConverter(APP_NAME);

    /// <summary>
    /// Receives stream data from the server.
    /// </summary>
    public override async Task<StreamState?> PreviewReceive(
        StreamState state, 
        ProgressViewModel progress)
    {
        try
        {
            IModelDoc2 activeDoc = App.IActiveDoc2;

            Commit commit = await ConnectorHelpers.GetCommitFromState(state, progress.CancellationToken);
            progress.Report = new ProgressReport();

            if (commit.id != SelectedReceiveCommit)
            {
                // check for converter 
                var converter = KitManager.GetDefaultKit().LoadConverter(APP_NAME);
                converter.SetContextDocument(activeDoc);

                var settings = new Dictionary<string, string>();
                CurrentSettings = state.Settings;
                foreach (var setting in state.Settings)
                    settings.Add(setting.Slug, setting.Selection);

                settings["preview"] = "true";
                converter.SetConverterSettings(settings);

                var commitObject = await ConnectorHelpers.ReceiveCommit(commit, state, progress);

                Preview.Clear();
                StoredObjects.Clear();

                Preview = FlattenCommitObject(commitObject, converter);

                foreach (var previewObj in Preview)
                    progress.Report.Log(previewObj);

                //Submit

            }
        }
        catch (Exception ex)
        {
            SpeckleLog.Logger.Error(
                ex,
                "Failed to preview receive",
                ex);
        }

        return state;
    }

    /// <summary>
    /// Receives stream data from the server.
    /// </summary>
    public override async Task<StreamState> ReceiveStream(
        StreamState state, 
        ProgressViewModel progress)
    {
        IModelDoc2 doc = App.IActiveDoc2;

        //make sure to instance a new copy so all values are reset correctly
        var converter = (ISpeckleConverter)Activator.CreateInstance(Converter.GetType());
        converter.SetContextDocument(doc);
        var previouslyReceiveObjects = state.ReceivedObjects;

        // set converter settings as tuples (setting slug, setting selection)
        var settings = new Dictionary<string, string>();
        CurrentSettings = state.Settings;
        foreach (var setting in state.Settings)
            settings.Add(setting.Slug, setting.Selection);
        converter.SetConverterSettings(settings);

        Commit myCommit = await ConnectorHelpers.GetCommitFromState(state, progress.CancellationToken);
        state.LastCommit = myCommit;
        Base commitObject = await ConnectorHelpers.ReceiveCommit(myCommit, state, progress);
        await ConnectorHelpers.TryCommitReceived(state, myCommit, APP_NAME, progress.CancellationToken);

        Preview?.Clear();
        StoredObjects.Clear();

        Preview = FlattenCommitObject(commitObject, converter);
        foreach (var previewObj in Preview)
            progress.Report.Log(previewObj);


        converter.ReceiveMode = state.ReceiveMode;
        // needs to be set for editing to work
        converter.SetPreviousContextObjects(previouslyReceiveObjects);
        // needs to be set for openings in floors and roofs to work
        converter.SetContextObjects(Preview);

        try
        {
            //await RevitTask.RunAsync(() => UpdateForCustomMapping(state, progress, myCommit.sourceApplication));
        }
        catch (Exception ex)
        {
            SpeckleLog.Logger.Warning(ex, "Could not update receive object with user types");
            progress.Report.LogOperationError(new Exception("Could not update receive object with user types. Using default mapping.", ex));
        }


        try
        {
            converter.SetContextDocument(doc);

            //var newPlaceholderObjects = ConvertReceivedObjects(converter, progress);

            //if (state.ReceiveMode == ReceiveMode.Update)
                //DeleteObjects(previouslyReceiveObjects, newPlaceholderObjects);

            //state.ReceivedObjects = newPlaceholderObjects;
        }
        catch (Exception ex)
        {
            SpeckleLog.Logger.Error("Error ReceiveStream", ex);

            string message = $"Fatal Error: {ex.Message}";
            if (ex is OperationCanceledException) message = "Receive cancelled";
            progress.Report.LogOperationError(new Exception($"{message} - Changes have been rolled back", ex));
        }

        return state;
    }

    #region Private Methods
    private List<ApplicationObject> FlattenCommitObject(Base obj, ISpeckleConverter converter)
    {
        ApplicationObject? CreateApplicationObject(Base current)
        {
            if (!converter.CanConvertToNative(current)) return null;

            var appObj = new ApplicationObject(
                current.id,
                current.speckle_type)
            {
                applicationId = current.applicationId,
                Convertible = true
            };
            if (StoredObjects.ContainsKey(current.id))
                return null;

            StoredObjects.Add(current.id, current);
            return appObj;
        }

        var traverseFunction = DefaultTraversal.CreateRevitTraversalFunc(converter);

        var objectsToConvert = traverseFunction.Traverse(obj)
          .Select(tc => CreateApplicationObject(tc.current))
          .Where(appObject => appObject != null)
          .Reverse()
          .ToList();

#pragma warning disable CS8619 
        return objectsToConvert;
#pragma warning restore CS8619 
    }

    #endregion
}
