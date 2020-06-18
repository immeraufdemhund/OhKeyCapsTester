using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using NUnit.Framework;
using OhKeyCapsTester.Contracts;

namespace OhKeyCapsTester.Tests.Core
{
    [TestFixture]
    public class KeyboardReaderServiceTests
    {
        private IKeyboardReaderService _instanceUnderTest;

        [OneTimeSetUp]
        protected static void OneTimeSetup()
        {
            IocContainer.Build();
        }

        [SetUp]
        protected void Setup()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            _instanceUnderTest = IocContainer.BaseContainer.Resolve<IKeyboardReaderService>();
        }

        [Test]
        public void TheTruth()
        {
            Assert.That(_instanceUnderTest, Is.Not.Null);
        }

        [Test]
        public void GetsLayouts()
        {
            var layouts = _instanceUnderTest.LoadKeyboardLayouts().ToList();

            Assert.That(layouts, Is.Not.Empty);
        }

        [Test]
        public void GetsLayoutManuform4x5()
        {
            var layouts = _instanceUnderTest.LoadKeyboardLayouts().ToList();
            var manuform = layouts.Single(x => x.KeyboardName == "Dactyl Manuform 4x5");

            Assert.That(manuform.Width, Is.EqualTo(15));
            Assert.That(manuform.Height, Is.EqualTo(7));
            Assert.That(manuform.Layouts.Count, Is.EqualTo(1));
            var layout = manuform.Layouts[0];
            Assert.That(layout.Name, Is.EqualTo("LAYOUT"));
            Assert.That(layout.Layout.Length, Is.EqualTo(46));
            var L00 = layout.Layout.Single(x => x.Label == "L00");
            Assert.That(L00, Is.EqualTo(new KeyLayoutCoordinate{Label = "L00", X = 0, Y = 0, H = 1}));
            var R00 = layout.Layout.Single(x => x.Label == "R00");
            Assert.That(R00, Is.EqualTo(new KeyLayoutCoordinate{Label = "R00", X = 10, Y = 0, H = 1}));
            var L44 = layout.Layout.Single(x => x.Label == "L44");
            Assert.That(L44, Is.EqualTo(new KeyLayoutCoordinate{Label = "L44", X = 5, Y = 5, H = 1}));
            var R43 = layout.Layout.Single(x => x.Label == "R43");
            Assert.That(R43, Is.EqualTo(new KeyLayoutCoordinate{Label = "R43", X = 8, Y = 6, H = 1}));
        }

        [Test]
        public void GetsHeight()
        {
            var layouts = _instanceUnderTest.LoadKeyboardLayouts().ToList();
            var dactyl = layouts.Single(x => x.KeyboardName == "Dactyl ProMicro");

            var layout = dactyl.Layouts[0];
            var L45 = layout.Layout.Single(x => x.Label == "L45");
            Assert.That(L45, Is.EqualTo(new KeyLayoutCoordinate{Label = "L45", X = 5, Y = 6, H = 2}));
            var R40 = layout.Layout.Single(x => x.Label == "R40");
            Assert.That(R40, Is.EqualTo(new KeyLayoutCoordinate{Label = "R40", X = 11, Y = 6, H = 2}));
            var L55 = layout.Layout.Single(x => x.Label == "L55");
            Assert.That(L55, Is.EqualTo(new KeyLayoutCoordinate{Label = "L55", X = 6, Y = 6, H = 2}));
            var R50 = layout.Layout.Single(x => x.Label == "R50");
            Assert.That(R50, Is.EqualTo(new KeyLayoutCoordinate{Label = "R50", X = 10, Y = 6, H = 2}));
        }

        [Test]
        public void GetsKeyboardRowsAndColumns()
        {
            void AssertRowAndCol(IEnumerable<KeyboardLayout> keyboards, string name, int expectedRows, int expectedCols)
            {
                var keyboard = keyboards.Single(x => x.KeyboardName == name);
                Assert.That(keyboard.Rows, Is.EqualTo(expectedRows), "you have the wrong row count for {0}", name);
                Assert.That(keyboard.Cols, Is.EqualTo(expectedCols), "you have the wrong column count for {0}", name);
            }

            var layouts  = _instanceUnderTest.LoadKeyboardLayouts().ToList();

            AssertRowAndCol(layouts, "Dactyl Manuform 4x5", 10, 5);
            AssertRowAndCol(layouts, "Dactyl Manuform 4x6", 10, 6);
            AssertRowAndCol(layouts, "Dactyl Manuform 5x6", 12, 6);
            AssertRowAndCol(layouts, "Dactyl Manuform 5x7", 12, 7);
            AssertRowAndCol(layouts, "Dactyl Manuform 6x6", 14, 6);
            AssertRowAndCol(layouts, "Dactyl ProMicro", 12, 6);
        }
    }
}
