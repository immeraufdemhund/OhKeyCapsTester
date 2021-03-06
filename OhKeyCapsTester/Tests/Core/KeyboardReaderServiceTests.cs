﻿using System.IO;
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
            Assert.That(L00, Is.EqualTo(new KeyLayoutCoordinate{Label = "L00", X = 0, Y = 0}));
            var R00 = layout.Layout.Single(x => x.Label == "R00");
            Assert.That(R00, Is.EqualTo(new KeyLayoutCoordinate{Label = "R00", X = 10, Y = 0}));
            var L44 = layout.Layout.Single(x => x.Label == "L44");
            Assert.That(L44, Is.EqualTo(new KeyLayoutCoordinate{Label = "L44", X = 5, Y = 5}));
            var R43 = layout.Layout.Single(x => x.Label == "R43");
            Assert.That(R43, Is.EqualTo(new KeyLayoutCoordinate{Label = "R43", X = 8, Y = 6}));
        }
    }
}
