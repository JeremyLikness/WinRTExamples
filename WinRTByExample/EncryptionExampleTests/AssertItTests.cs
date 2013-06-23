// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertItTests.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The assert it tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExampleTests
{
    using FluentTestHelper;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    /// <summary>
    /// The assert that tests.
    /// </summary>
    [TestClass]
    public class AssertItTests
    {
        /// <summary>
        /// The given an integer when compared to a greater integer then is greater than should fail.
        /// </summary>
        [TestMethod]
        public void GivenAnIntegerWhenComparedGreatherThanToAGreaterIntegerThenShouldFail()
        {
            var exception = false;

            try
            {
                AssertIt.That(2, Is.GreaterThan(3));                
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given an integer when compared to a greater integer then is greater than should fail.
        /// </summary>
        [TestMethod]
        public void GivenAnIntegerWhenComparedGreaterThanToALesserIntegerThenShouldSucceed()
        {
            AssertIt.That(3, Is.GreaterThan(2));            
        }

        /// <summary>
        /// The given an empty string when checked for not null or whitespace then should fail.
        /// </summary>
        [TestMethod]
        public void GivenAnEmptyStringWhenCheckedForNotNullOrWhitespaceThenShouldFail()
        {
            var exception = false;

            try
            {
                AssertIt.That(string.Empty, Is.NotNullOrWhitespace());
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given an string when checked for not null or whitespace then should succeed.
        /// </summary>
        [TestMethod]
        public void GivenANonEmptyStringWhenCheckedForNotNullOrWhitespaceThenShouldSucceed()
        {
            AssertIt.That("Test", Is.NotNullOrWhitespace());
        }

        /// <summary>
        /// The given true when compared to false then should fail
        /// </summary>
        [TestMethod]
        public void GivenTrueWhenFalseCheckedThenShouldFail()
        {
            var exception = false;

            try
            {
                AssertIt.That(true, Is.False);
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given false when compared to false then should succeed
        /// </summary>
        [TestMethod]
        public void GivenFalseWhenFalseCheckedThenShouldSucceed()
        {
            AssertIt.That(false, Is.False);
        }

        /// <summary>
        /// The given false when compared to true then should fail
        /// </summary>
        [TestMethod]
        public void GivenFalseWhenTrueCheckedThenShouldFail()
        {
            var exception = false;

            try
            {
                AssertIt.That(false, Is.True);
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given true when compared to true then should succeed
        /// </summary>
        [TestMethod]
        public void GivenTrueWhenTrueCheckedThenShouldSucceed()
        {
            AssertIt.That(true, Is.True);
        }

        /// <summary>
        /// The given null object when checked for not null then should fail.
        /// </summary>
        [TestMethod]
        public void GivenNullObjectWhenCheckedForNotNullThenShouldFail()
        {
            var exception = false;

            try
            {
                AssertIt.That(null, Is.NotNull<object>());
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given true when compared to true then should succeed
        /// </summary>
        [TestMethod]
        public void GivenNonNullObjectWhenCheckedForNotNullThenShouldSucceed()
        {
            AssertIt.That(new object(), Is.NotNull<object>());
        }

        /// <summary>
        /// The given an integer when compared equal to a different integer then should fail.
        /// </summary>
        [TestMethod]
        public void GivenAnIntegerWhenComparedEqualToADifferentIntegerThenShouldFail()
        {
            var exception = false;

            try
            {
                AssertIt.That(2, Is.EqualTo(3));
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given an integer when compared equal to the same integer then should succeed.
        /// </summary>
        [TestMethod]
        public void GivenAnIntegerWhenComparedEqualToTheSameIntegerThenShouldSucceed()
        {
            AssertIt.That(2, Is.EqualTo(2));
        }

        /// <summary>
        /// The given collection with unwanted item when only called then should fail 
        /// </summary>
        [TestMethod]
        public void GivenCollectionWithUnwantedItemWhenOnlyCalledThenShouldFail()
        {
            var exception = false;

            try
            {
                var test = new[] { "abc", "def", "gh" };
                AssertIt.That(test, Has.Only<string[], string>(str => str.Length.IsEqualTo(3)));
            }
            catch (AssertFailedException)
            {
                exception = true;
            }

            Assert.IsTrue(exception, "Test failed: an exception should have been thrown.");
        }

        /// <summary>
        /// The given collection with no unwanted items when only called then should succeed 
        /// </summary>
        [TestMethod]
        public void GivenCollectionWithNoUnwantedItemsWhenOnlyCalledThenShouldSucceed()
        {
            var test = new[] { "abc", "def", "ghi" };
            AssertIt.That(test, Has.Only<string[], string>(str => str.Length.IsEqualTo(3)));
        }
    }
}