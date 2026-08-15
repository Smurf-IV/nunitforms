#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
' 
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
' 
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
' 
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
' 
' * Neither the name of the author nor the names of its contributors may be used to endorse or 
' 	promote products derived from this software without specific prior written permission.
' 
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

using NUnit.Extensions.Forms.Recorder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.TestApplications
{
    [TestFixture]
    public class FormFinderTest : NUnitFormTest
    {
        private FormFinder finder;

        public override void Setup()
        {
            finder = new FormFinder();
        }

        private Form ShowNewForm(string name)
        {
            Form form = new Form();
            form.Name = name;
            form.Show();
            return form;
        }

        [Test]
        public void FindAll()
        {
            Form one = ShowNewForm("form");
            Form two = ShowNewForm("form2");
            Form three = ShowNewForm("form3");
            List<Form> found = finder.FindAll();
            Assert.AreEqual(3, found.Count);
            Assert.IsTrue(found.Contains(one));
            Assert.IsTrue(found.Contains(two));
            Assert.IsTrue(found.Contains(three));
        }

        [Test]
        public void FinderWithBadObjectHasNoName()
        {
            var ex = Assert.Throws<Exception>(() => { new Finder<Control>().Name("a"); });
            Assert.That(ex.Message, Does.Contain("Object name not defined"));
        }

        [Test]
        public void FindOneForm()
        {
            Form form = ShowNewForm("form");
            Form found = finder.Find("form");
            Assert.AreSame(form, found);
        }

        [Test]
        public void FindOneFormOutOfTwo()
        {
            Form one = ShowNewForm("form");
            Form two = ShowNewForm("form2");
            Assert.AreEqual(2, finder.FindAll().Count);

            Form found = finder.Find("form");
            Assert.AreSame(one, found);
            found = finder.Find("form2");
            Assert.AreSame(two, found);
        }

        [Test]
        public void FindOneFormWhenThereAreNone()
        {
            var ex = Assert.Throws<NoSuchControlException>(() => finder.Find("form"));
            Assert.That(ex.Message, Does.Contain("Could not find form with name 'form'"));
        }

        [Test]
        public void FindOneFormWhenThereAreTwo()
        {
            ShowNewForm("form");
            ShowNewForm("form");
            var ex = Assert.Throws<AmbiguousNameException>(() => finder.Find("form"));
            Assert.That(ex.Message, Does.Contain("Found too many forms with the name 'form'"));
        }

        [Test]
        public void FindTwoFormsWhenThereAreTwo()
        {
            Form one = ShowNewForm("form");
            Form two = ShowNewForm("form");
            List<Form> found = finder.FindAll("form");
            Assert.AreEqual(2, found.Count);
            Assert.IsTrue(found.Contains(one));
            Assert.IsTrue(found.Contains(two));
        }
    }
}