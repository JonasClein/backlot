using System;
using System.Collections.Generic;
using Backlot.Core;
using Backlot.Core.Abstraction.Actors;
using Backlot.Core.Json;
using Backlot.Core.Security;
using Backlot.Testing.Core;
using Backlot.Testing.Shared.Stubs;
using Backlot.Testing.Shared.TestModels;
using NUnit.Framework;

namespace Backlot.Testing;

/*
 * DEFINITIONS;
 * Namingconvention; MethodNameToTest_StateUnderTest_ExpectedBehavior
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  
 * Dummy - objects are passed around but never actually used. Usually they are just used to fill parameter lists.
 * Fake - objects actually have working implementations, but usually take some shortcut which makes them not suitable for production (an in memory database is a good example).
 * Stubs - provide canned answers to calls made during the test, usually not responding at all to anything outside what's programmed in for the test. Stubs may also record information about calls, such as an email gateway stub that remembers the messages it 'sent', or maybe only how many messages it 'sent'.
 * Mocks - are what we are talking about here: objects pre-programmed with expectations which form a specification of the calls they are expected to receive
 */

public class PermissionControl
{

    [SetUp]
    public void Setup()
    {
        Initialize.Setup();
    }
    
    [Test]
    public void ManagePermission_Example_Debugging()
    {
        
        var typed = Acting.New<IFormula>();
        typed.Number1 = 7;
        typed.Number2 = 9;
        typed.Operation = "sum";

        typed.ManagePermission(p => p
            .SetMask(PermissionLevel.ReadWriteExecute)
            .Clear() // clear all other permissions and 
            .SetGroup(Access.Open, PermissionLevel.Read)
            .SetUser("TEST", PermissionLevel.ReadWriteExecute)
            .SetGroup(Access.Admin, PermissionLevel.ReadWrite));

        Assert.That(typed.CanWrite());
        Assert.That(!typed.CanExecute());;
    }
}
