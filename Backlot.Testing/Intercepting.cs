using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Json;
using Backlot.Testing.Core;
using Backlot.Testing.Shared.TestModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Backlot.Testing;

/// <summary>
/// Test to check if intercepting using the backlot interceptors works correctly.
/// </summary>
public class Intercepting
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup();
    }

    [Test]
    public void Intercepting_WithJSONACTOR_ActorAndProxiedValuesAreInSync()
    {
        #region ARRANGE

        string uid = "023d02600ded";
        
        var origin = JObject.FromObject(new
        {
            Uid = uid,
            Number1 = 7,
            Number2 = 9,
            Operation = "sum"
        });
        
        var role = origin.Presents<IFormula>();
        // ReSharper disable once SuspiciousTypeConversion.Global; within this unit test always an IJProxy.
        var jProxy = (IJProxy)role;

        #endregion

        Assert.That(jProxy.JActor["Number1"]?.Value<int?>() == 7);
        Assert.That(role.Number1 == 7);

        role.Number1 = 8;
        
        // because json origins do work as a cloned json container the original object is not updated, however the "cloned" Origin is.
        
        // with json objects the the role and the "origin"/Actor are updated, the original object is not updated, because it is deepcloned
        // json objects do work "by 'value'".
        
        Assert.That(role.Number1 == 8);
        Assert.That(jProxy.JActor["Number1"]?.Value<int?>() == 8); 
        Assert.That(origin["Number1"]?.Value<int?>() == 8, Is.False,"JsonInterception does work with DeepCloned objects");
    }
    
    
    [Test]
    public void Intercepting_WithJSONNestedRoleACTOR_ActorAndProxiedValuesAreInSync()
    {
        #region ARRANGE

        string uid = "5DEB1636";

        var origin = JObject.FromObject(new
        {
            Uid = uid,
            Formulas = new [] { 
                new {
                    Number1 = 7,
                    Number2 = 9,
                    Operation = "sum"
                },
                new {
                    Number1 = 17,
                    Number2 = 19,
                    Operation = "sum"
                }
            }
        });
        
        
        var role = origin.Presents<IFormulaGroup>();
        // ReSharper disable once SuspiciousTypeConversion.Global; within this unit test always an IJProxy.
        var jProxy = (IJProxy)role;

        #endregion

        Assert.That(jProxy.JActor["Formulas"][0]["Number1"]?.Value<int?>() == 7);
        Assert.That(role.Formulas.First().Number1 == 7);

        role.Formulas.First().Number1 = 8;
        
        Assert.That(origin["Formulas"][0]["Number1"]?.Value<int?>() == 8, Is.False);
        Assert.That(jProxy.JActor["Formulas"][0]["Number1"]?.Value<int?>() == 8);

    }
    
    [Test]
    public void Intercepting_WithJSONNestedTypedACTOR_ActorAndProxiedValuesAreInSync()
    {
        #region ARRANGE

        string uid = "5DEB1636";

        var origin = JObject.FromObject(new
        {
            Uid = uid,
            Formulas = new [] { 
                new {
                    Number1 = 7,
                    Number2 = 9,
                    Operation = "sum"
                },
                new {
                    Number1 = 17,
                    Number2 = 19,
                    Operation = "sum"
                }
            }
        });
        
        
        var role = origin.Presents<IFormulaGroupTyped>();
        // ReSharper disable once SuspiciousTypeConversion.Global; within this unit test always an IJProxy.
        var jProxy = (IJProxy)role;

        #endregion

        Assert.That(jProxy.JActor["Formulas"][0]["Number1"]?.Value<int?>() == 7);
        var f = role.Formulas.First();
        Assert.That(f.Number1 == 7);

        role.Formulas.First().Number1 = 8;

        Assert.That(origin["Formulas"][0]["Number1"]?.Value<int?>() == 8, Is.False);
        Assert.That(jProxy.JActor["Formulas"][0]["Number1"]?.Value<int?>() == 8);
    }
    
    [Test]
    public void Intercepting_WithJSONNestedSelfACTOR_ActorAndProxiedValuesAreInSync()
    {
        #region ARRANGE

        string uid = "5DEB1636";

        var origin = JObject.FromObject(new
        {
            Uid = uid,
            Formulas = new [] { 
                new {
                    Number1 = 7,
                    Number2 = 9,
                    Operation = "sum"
                },
                new {
                    Number1 = 17,
                    Number2 = 19,
                    Operation = "sum"
                }
            }
        });
        
        
        var role = origin.Presents<IFormulaGroupSelf>();
        // ReSharper disable once SuspiciousTypeConversion.Global; within this unit test always an IJProxy.
        var jProxy = (IJProxy)role;

        #endregion
        
        Assert.That(jProxy.JActor["Formulas"][0]["Number1"]?.Value<int?>() == 7);
        Assert.That(role.Formulas.First().Number1 == 7);

        role.Formulas.First().Number1 = 8;

        Assert.That(origin["Formulas"][0]["Number1"]?.Value<int?>() == 8, Is.False);
        Assert.That(jProxy.JActor["Formulas"][0]["Number1"]?.Value<int?>() == 8);
    }
}