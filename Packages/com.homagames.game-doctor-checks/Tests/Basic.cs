using System;
using System.Linq;
using HomaGames.GameDoctor.Checks;
using HomaGames.GameDoctor.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Tests
{
    public class BasicTests
    {
        [Test]
        public void ImplementedChecksAreRegistered()
        {
            var type = typeof(ICheck);
            var types = typeof(ChecksRegistration).Assembly.GetTypes()
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract).ToList();
            var allChecks = AvailableChecks.GetAllChecks();
            foreach (var VARIABLE in types)
            {
                Debug.Log(VARIABLE.FullName);
            }
            foreach (var VARIABLE in allChecks)
            {
                Debug.Log(VARIABLE.GetType().FullName);
            }
            Assert.True(types.Count()==allChecks.Count,$"{types.Count} checks found but {allChecks.Count} checks registered.");
        }
    }
}