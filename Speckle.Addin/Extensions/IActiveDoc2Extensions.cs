﻿using System;
using System.Collections.Generic;

namespace SolidWorks.Interop.sldworks
{
    /// <summary>
    /// Extension Methods for <see cref="GetTopFeatures(IModelDoc2)"/> Interface
    /// </summary>
    public static class IActiveDoc2Extensions
    {
        #region Feature
        public static IEnumerable<IFeature> GetAllFeatures(
            this IModelDoc2 doc)
        {
            var topFeats = doc.GetTopFeatures();
            foreach (var topFeat in topFeats)
            {
                yield return topFeat;

                foreach (var subTopFeat in topFeat.GetAllSubFeatures())
                {
                    yield return subTopFeat;                    
                }
            }
        }

        public static IEnumerable<IFeature> GetTopFeatures(
            this IModelDoc2 doc)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var feat = doc.FirstFeature() as IFeature;
            while (feat != null)
            {
                yield return feat;
                feat = feat.GetNextFeature() as IFeature;
            }
        }

        public static IEnumerable<IFeature> GetAllSubFeatures(
            this IFeature feat)
        {
            var subFeat = feat.GetFirstSubFeature() as IFeature;

            while (subFeat != null)
            {

                var subsubFeats = subFeat.GetAllSubFeatures();

                foreach (var subsubFeat in subsubFeats)
                {
                    yield return subsubFeat;
                }

                yield return subFeat;

                subFeat = subFeat.GetNextSubFeature() as IFeature;
            }
        }
        #endregion
    }
}
