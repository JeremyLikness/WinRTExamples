// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelTests.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExampleTests
{
    using System.Collections.Generic;
    using System.Linq;

    using EncryptionExample.Crypto;
    using EncryptionExample.Data;

    using FluentTestHelper;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    /// <summary>
    /// The view model tests.
    /// </summary>
    [TestClass]
    public class ViewModelTests
    {
        /// <summary>
        /// The target.
        /// </summary>
        private ViewModel target;

        /// <summary>
        /// The mock dialog service
        /// </summary>
        private MockDialog mockDialogService;

        /// <summary>
        /// The test initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.mockDialogService = new MockDialog();
            this.target = new ViewModel
                                {
                                    Dialog = this.mockDialogService
                                };
        }

        /// <summary>
        /// The given view model when created then list of algorithms should not be empty.
        /// </summary>
        [TestMethod]
        public void GivenViewModelWhenCreatedThenListOfAlgorithmsShouldNotBeEmpty()
        {
            AssertIt.That(this.target.Algorithms.Count(), Is.GreaterThan(0), "Test failed: list of algorithms should be initialized.");
        }

        /// <summary>
        /// The given view model when created then encrypt property should be populated.
        /// </summary>
        [TestMethod]
        public void GivenViewModelWhenCreatedThenEncryptPropertyShouldBePopulated()
        {
            AssertIt.That(this.target.EncryptionInput, Is.NotNullOrWhitespace(), "Test failed: encryption input should not be empty.");
        }

        /// <summary>
        /// The given view model when created then decrypt property should be populated.
        /// </summary>
        [TestMethod]
        public void GivenViewModelWhenCreatedThenDecryptPropertyShouldBePopulated()
        {
            AssertIt.That(this.target.DecryptionInput, Is.NotNullOrWhitespace(), "Test failed: decryption input should not be empty.");
        }

        /// <summary>
        /// The given view model when created then a selected algorithm should be set.
        /// </summary>
        [TestMethod]
        public void GivenViewModelWhenCreatedThenSelectedAlgorithmShouldBeSet()
        {
            AssertIt.That(this.target.SelectedAlgorithm, Is.NotNull<ICryptoAlgorithm>(), "Test failed: selected algorithm should not be null.");
        }

        /// <summary>
        /// The given null parameter when can execute called then should return false.
        /// </summary>
        [TestMethod]
        public void GivenNullParameterWhenCanExecuteCalledThenShouldReturnFalse()
        {
            AssertIt.That(this.target.CanExecute(null), Is.False, "Test failed: can execute should be false when parameter is null.");
        }

        /// <summary>
        /// The given non null parameter when can execute called then should return true.
        /// </summary>
        [TestMethod]
        public void GivenNonNullParameterWhenCanExecuteCalledThenShouldReturnTrue()
        {
            AssertIt.That(this.target.CanExecute("sign"), Is.True, "Test failed: can execute should be true when parameter is not whitespace.");
        }

        /// <summary>
        /// The given no algorithm selected when can execute called then should return false.
        /// </summary>
        [TestMethod]
        public void GivenNoAlgorithmSelectedWhenCanExecuteCalledThenShouldReturnFalse()
        {
            this.target.SelectedAlgorithm = null;
            AssertIt.That(this.target.CanExecute("sign"), Is.False, "Test failed: can execute should be false when no alogirthm is selected.");
        }

        /// <summary>
        /// The given can not execute when execute called then does nothing.
        /// </summary>
        [TestMethod]
        public void GivenCanNotExecuteWhenExecuteCalledThenDoesNothing()
        {
            this.target.SelectedAlgorithm = null;

            // will throw exception if it tries to actually call the sign action
            this.target.Execute("sign");            
        }

        /// <summary>
        /// The given no encryption input when execute encrypt called then shows dialog and returns.
        /// </summary>
        [TestMethod]
        public void GivenNoEncryptionInputWhenExecuteEncryptCalledThenShowsDialogAndReturns()
        {
            this.target.EncryptionInput = string.Empty;
            this.target.Execute("encrypt");
            AssertIt.That(
                this.mockDialogService.DialogCount, 
                Is.EqualTo(1), 
                "Test failed: dialog should have been called exactly once.");            
        }

        /// <summary>
        /// The given is symmetric filter selected when algorithm list requested then only contains symmetric algorithms.
        /// </summary>
        [TestMethod]
        public void GivenIsSymmetricFilterSelectedWhenAlgorithmListRequestedThenOnlyContainsSymmetricAlgorithms()
        {
            this.target.Symmetric = true;
            AssertIt.That(this.target.Algorithms.Count(), Is.GreaterThan(0), "Test failed: the filter should return some algorithms.");
            AssertIt.That(this.target.Algorithms, Has.Only<IEnumerable<ICryptoAlgorithm>, ICryptoAlgorithm>(crypto => crypto.IsSymmetric));
        }
    }
}
