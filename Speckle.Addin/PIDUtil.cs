using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace Speckle.Addin
{
    public static class PIDUtil
    {

        public static string GetPID(this IModelDoc2 doc, object obj)
        {
            var data = doc.Extension.GetPersistReference3(obj) as byte[];

            return data == null ? string.Empty : ByteArrayToString(data);
        }

        public static bool IsSamePID(this IModelDocExtension extension, string p1, string p2)
        {
            var res = (swObjectEquality)extension.IsSamePersistentID(Convert.FromBase64String(p1), Convert.FromBase64String(p2));
            return res == swObjectEquality.swObjectSame;
        }

        public static object GetObjectFromPID(this IModelDoc2 doc, string pid, out swPersistReferencedObjectStates_e state)
        {
            var byteId = Convert.FromBase64String(pid); //_encoder.GetBytes(pid);

            var obj = doc.Extension.GetObjectByPersistReference3(byteId, out int errorCode);
            state = (swPersistReferencedObjectStates_e)errorCode;

            return obj;
        }

        private static string ByteArrayToString(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length < 2)
            {
                return $"Byte Error {byteArray?.Length}";
            }

            string value = Convert.ToBase64String(byteArray);

            return value;
        }
    }
}