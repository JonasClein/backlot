using System;
using System.Threading.Tasks;
using Backlot.Core;
using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Abstraction.Roles;
using Backlot.Core.DependencyInjection;
using Backlot.Core.Json;
using Backlot.Core.Services;
using Backlot.Testing.Core;
using Backlot.Testing.Core.Loader;
using Backlot.Testing.Shared.Scenarios;
using Backlot.Testing.Shared.TestModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Backlot.Testing;

public class Reflection
{
    [SetUp]
    public void Setup()
    {
        Initialize.Setup();
    }

    [Test]
    public void GetRoleByName_InterfaceNameStartingWithI()
    {
        #region ARRANGE

        var correctRoleName = "InterfaceStartingWithI";
        var correctRoleName_Lowercase = "interfacestartingwithi";
        var correctRoleName_Uppercase = "INTERFACESTARTINGWITHI";

        var incorrectRoleName_ExtraI = "IInterfaceStartingWithI";
        var incorrectRoleName_MissingI = "nterfaceStartingWithI";
        var incorrectRoleName_WrongInterface = "Formula";

        var interfaceTypeToCheck = typeof(IInterfaceStartingWithI);

        #endregion

        Assert.That(GetByNameResultIsTypeof(correctRoleName, interfaceTypeToCheck));
        Assert.That(GetByNameResultIsTypeof(correctRoleName_Lowercase, interfaceTypeToCheck));
        Assert.That(GetByNameResultIsTypeof(correctRoleName_Uppercase, interfaceTypeToCheck));
        Assert.That(GetByNameResultIsTypeof(incorrectRoleName_ExtraI, interfaceTypeToCheck));
        
        Assert.That(GetByNameResultIsTypeof(incorrectRoleName_MissingI, interfaceTypeToCheck), Is.False);
        Assert.That(GetByNameResultIsTypeof(incorrectRoleName_WrongInterface, interfaceTypeToCheck), Is.False);
    }

    [Test]
    public void GetRoleByName_IFormula()
    {
        #region ARRANGE

        var correctRoleName = "Formula";
        var correctRoleName_Lowercase = "formula";
        var correctRoleName_Uppercase = "FORMULA";

        var interfacename = "IFormula";
        var incorrectRoleName_ImplementingClass = "FormulaSelf";
        var incorrectRoleName_WrongInterface = "Director";

        var interfaceTypeToCheck = typeof(IFormula);

        #endregion

        Assert.That(GetByNameResultIsTypeof(correctRoleName, interfaceTypeToCheck));
        Assert.That(GetByNameResultIsTypeof(correctRoleName_Lowercase, interfaceTypeToCheck));
        Assert.That(GetByNameResultIsTypeof(correctRoleName_Uppercase, interfaceTypeToCheck));
        Assert.That(GetByNameResultIsTypeof(interfacename, interfaceTypeToCheck));
        
        Assert.That(GetByNameResultIsTypeof(incorrectRoleName_ImplementingClass, interfaceTypeToCheck), Is.False);
        Assert.That(GetByNameResultIsTypeof(incorrectRoleName_WrongInterface, interfaceTypeToCheck), Is.False);
    }

    private static bool GetByNameResultIsTypeof(string name, Type type)
    {
        if (Loader.TryGetRoleByName(name, out var t))
        {
            return t == type;
        }

        return false;
    }

    [Test]
    public void GetRoleByName_ReturnsNullIfNotExist()
    {
        var uid = Uid.New();
        var nonExistingRoleName = "ThisInterfaceWillNeverExistInBacklot" + uid;

        Assert.That(Loader.TryGetRoleByName(nonExistingRoleName, out _), Is.False);
    }

}