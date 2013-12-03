using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace PortableTests
{
    using PortableMVVM;

    [TestClass]
    public class RunOnceCommandTests
    {
        [TestMethod]
        public void GivenNeverExecutedWhenCanExecuteCalledThenShouldBeTrue()
        {
            var target = new RunOnceCommand(() => { });
            Assert.IsTrue(
                target.CanExecute(null),
                "Test failed: can execute should return true when command has not been executed.");
        }

        [TestMethod]
        public void GivenExecutedOnceWhenCanExecuteCalledThenShouldBeFalse()
        {
            var target = new RunOnceCommand(() => { });
            target.Execute(null);
            Assert.IsFalse(
                target.CanExecute(null),
                "Test failed: can execute should return false when command has been executed.");
        }
    }
}
