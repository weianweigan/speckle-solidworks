using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;

namespace Speckle.ConnectorSolidWorks.Selection
{
    /// <summary>
    /// Extension Methods for SelectionMgr
    /// </summary>
    public static class ISelectionMgrExtension
    {
        /// <summary>
        /// Add to current selectionSet without refresh
        /// </summary>
        public static void AddToCurrentSelectionSet(
            this ISelectionMgr selMgr,
            int mark,
            params object[] objs)
        {
            var swSelData = selMgr.CreateSelectData();
            swSelData.Mark = mark;

            foreach (var obj in objs)
            {
                selMgr.AddSelectionListObject(obj, swSelData);
            }
        }

        public static void WithSuspend(this ISelectionMgr seleMgr, Action action)
        {
            seleMgr.SuspendSelectionList();

            try
            {
                action?.Invoke();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                seleMgr.ResumeSelectionList();
            }
        }

        /// <summary>
        /// Add to current selectionSet without refresh
        /// </summary>
        public static void AddToCurrentSelectionSet(
            this ISelectionMgr selMgr,
            params KeyValuePair<int, object>[] objs)
        {
            foreach (var obj in objs)
            {
                var swSelData = selMgr.CreateSelectData();
                swSelData.Mark = obj.Key;

                selMgr.AddSelectionListObject(obj, swSelData);
            }
        }

        public static IEnumerable<SwSeleTypeObjectPair> GetSelections(
            this ISelectionMgr seleMgr)
        {
            var count = seleMgr.GetSelectedObjectCount2(-1);
            for (int i = 1; i < count + 1; i++)
            {
                var mark = seleMgr.GetSelectedObjectMark(i);
                var type = seleMgr.GetSelectedObjectType3(i, mark);
                var obj = seleMgr.GetSelectedObject6(i, mark);
                var postion = seleMgr.GetSelectionPoint2(i, mark) as double[];

                yield return new SwSeleTypeObjectPair(
                    i,
                    (swSelectType_e)type,
                    mark,
                    obj,
                    "",
                    postion);
            }
        }

        public static TSelection GetLastSelection<TSelection>(
            this ISelectionMgr seleMgr)
            where TSelection : class
        {
            var count = seleMgr.GetSelectedObjectCount2(-1);
            var mark = seleMgr.GetSelectedObjectMark(count);
            var obj = seleMgr.GetSelectedObject6(count, mark);

            return obj as TSelection;
        }

        public static IEnumerable<swSelectType_e> GetTypes(
            this ISelectionMgr seleMgr)
        {
            var count = seleMgr.GetSelectedObjectCount2(-1);
            for (int i = 1; i < count + 1; i++)
            {
                var mark = seleMgr.GetSelectedObjectMark(i);
                var type = seleMgr.GetSelectedObjectType3(i, mark);

                yield return (swSelectType_e)type;
            }
        }

        public static IEnumerable<object> GetObjs(
            this ISelectionMgr seleMgr)
        {
            var count = seleMgr.GetSelectedObjectCount2(-1);
            for (int i = 1; i < count + 1; i++)
            {
                var mark = seleMgr.GetSelectedObjectMark(i);
                var obj = seleMgr.GetSelectedObject6(i, mark);
                var type = seleMgr.GetSelectedObjectType3(i, mark);

                yield return obj;
            }
        }

        public static IEnumerable<object> GetObjs(
            this ISelectionMgr seleMgr,
            swSelectType_e neededType)
        {
            var count = seleMgr.GetSelectedObjectCount2(-1);
            for (int i = 1; i < count + 1; i++)
            {
                var mark = seleMgr.GetSelectedObjectMark(i);
                var obj = seleMgr.GetSelectedObject6(i, mark);
                var type = seleMgr.GetSelectedObjectType3(i, mark);

                if (type == (int)neededType)
                {
                    yield return obj;
                }
            }
        }
    }
}
