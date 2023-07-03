using System;
using System.Collections.Generic;
using System.Reflection;

namespace HomaGames.HomaBelly.GameDoctor
{
    public class MethodDescription
    {
        private readonly Type _type;
        private readonly string _method;
        private List<MethodInfo> _methodInfos;
        public readonly string MainDocumentationLink;
        public readonly string AdditionalDescription;
        private HashSet<string> _discardedNamespaces;

        public MethodDescription(Type type, HashSet<string> discardedNamespaces, string method,
            string mainDocumentationLink,
            string additionalDescription = "")
        {
            _type = type;
            _method = method;
            MainDocumentationLink = mainDocumentationLink;
            AdditionalDescription = additionalDescription;
            _discardedNamespaces = discardedNamespaces;
        }

        private List<MethodInfo> MethodInfos
        {
            get
            {
                if (_methodInfos != null || _type == null) return _methodInfos;

                _methodInfos =
                    new List<MethodInfo>(_type.GetMethods(BindingFlags.Public | BindingFlags.Static |
                                                          BindingFlags.Instance | BindingFlags.NonPublic));
                var methodName = _method;
                _methodInfos.RemoveAll(m => m.Name != methodName);

                return _methodInfos;
            }
        }

        public string MethodName => _method;

        public string FullMethodName => (MethodInfos != null && MethodInfos.Count > 0)
            ? $"{MethodInfos[0].DeclaringType.FullName}.{MethodName}"
            : MethodName;

        private bool IsDiscardedCall(MethodBase methodBase)
        {
            string declaringType = methodBase.DeclaringType == null ? "" : methodBase.DeclaringType.FullName;
            foreach (var namespaceString in _discardedNamespaces)
            {
                if (declaringType.Contains(namespaceString))
                    return true;
            }

            return false;
        }

        public List<MethodBase> GetUsages()
        {
            var allUsages = new List<MethodBase>();
            if (MethodInfos == null) return allUsages;
            foreach (var methodInfo in MethodInfos)
            {
                var usages = CodeAnalyzer.CodeAnalyzer.FindUsageOfMethod(methodInfo, typeof(HomaBelly).Assembly);
                usages.RemoveAll(IsDiscardedCall);
                allUsages.AddRange(usages);
            }

            return allUsages;
        }
    }
}