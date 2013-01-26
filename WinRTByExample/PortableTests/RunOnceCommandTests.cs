// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunOnceCommandTests.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The run once command tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortableTests
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using PortableMvvm;

    /// <summary>
    /// The run once command tests.
    /// </summary>
    [TestClass]
    public class RunOnceCommandTests
    {
        /// <summary>
        /// The given never executed when can execute called then should be true test.
        /// </summary>
        [TestMethod]
        public void GivenNeverExecutedWhenCanExecuteCalledThenShouldBeTrue()
        {
            var target = new RunOnceCommand(() => { });
            Assert.IsTrue(
                target.CanExecute(null),
                "Test failed: can execute should return true when command has not been executed.");
        }

        /// <summary>
        /// The given executed when can execute called then should be false test.
        /// </summary>
        [TestMethod]
        public void GivenExecutedWhenCanExecuteCalledThenShouldBeFalse()
        {
            var target = new RunOnceCommand(() => { });
            target.Execute(null);
            Assert.IsFalse(
                target.CanExecute(null),
                "Test failed: can execute should return false when command has been executed.");
        }

        /// <summary>
        /// The given command when executed then should execute delegate passed in test.
        /// </summary>
        [TestMethod]
        public void GivenCommandWhenExecutedThenShouldExecuteDelegatePassedIn()
        {
            bool delegateCalled = false;
            var target = 
                new RunOnceCommand(() => { delegateCalled = true; });
            target.Execute(null);
            Assert.IsTrue(
                delegateCalled,
                "Test failed: can execute should call the delegate passed in.");
        }
    }
}
