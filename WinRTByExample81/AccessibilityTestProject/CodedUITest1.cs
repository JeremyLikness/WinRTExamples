using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Input;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting.DirectUIControls;
using Microsoft.VisualStudio.TestTools.UITesting.WindowsRuntimeControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace AccessibilityTestProject
{
    using System.Reflection;
    using System.Security.Principal;
    using Microsoft.VisualStudio.TestTools;

    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest(CodedUITestType.WindowsStore)]
    public class CodedUITest1
    {
        public CodedUITest1()
        {
        }

        [TestMethod]
        public void CodedUITestMethod1()
        {
            Microsoft.VisualStudio.TestTools.UITest.Input.Point pt;
            
            var myapp = XamlWindow.Launch("WinRTByExampleAccessibilityExample_req6rhny9ggkj!App");
            Gesture.Tap(this.UIMap.UIAccessibilityExampleWindow.UIAgeEdit);
            // should fail because no error message yet
            Assert.IsFalse(this.UIMap.UIAccessibilityExampleWindow.UINameisRequiredText.TryGetClickablePoint(out pt));

            this.UIMap.UIAccessibilityExampleWindow.UIAgeEdit.Text = "32";
            Keyboard.SendKeys("s", ModifierKeys.Control); // submission 

            // should be true now
            Assert.IsTrue(this.UIMap.UIAccessibilityExampleWindow.UINameisRequiredText.TryGetClickablePoint(out pt));

            Gesture.Tap(this.UIMap.UIAccessibilityExampleWindow.UINameEdit);
            Keyboard.SendKeys("Name");
            Keyboard.SendKeys("s", ModifierKeys.Control); // submission 

            // should fail because error message cleared
            Assert.IsFalse(this.UIMap.UIAccessibilityExampleWindow.UINameisRequiredText.TryGetClickablePoint(out pt));
            myapp.Close();
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
