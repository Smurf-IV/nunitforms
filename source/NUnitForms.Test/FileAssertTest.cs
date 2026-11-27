#region Copyright (c) 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Modernisation 2026-2026 Smurf.IV
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
' ******************************************************************************************************************/

#endregion

using System.IO;

using NUnit.Framework;


namespace NUnit.WinForms.Test;

[TestFixture]
public class FileAssertTest
{
    private string _tempDir = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Test]
    public void NonExistentFilesReturnFalse()
    {
        string file1 = Path.Combine(_tempDir, "file1.bin");
        string file2 = Path.Combine(_tempDir, "file2.bin");

        Assert.IsFalse(FileAssert.AreBinaryEqual(file1, file2));

        File.WriteAllBytes(file1, [1, 2, 3]);
        Assert.IsFalse(FileAssert.AreBinaryEqual(file1, file2));
        Assert.IsFalse(FileAssert.AreBinaryEqual(file2, file1));
    }

    [Test]
    public void IdenticalFilesReturnTrue()
    {
        string file1 = Path.Combine(_tempDir, "file1.bin");
        string file2 = Path.Combine(_tempDir, "file2.bin");

        byte[] bytes = [1, 2, 3, 4, 5, 255, 0, 128];
        File.WriteAllBytes(file1, bytes);
        File.WriteAllBytes(file2, bytes);

        Assert.IsTrue(FileAssert.AreBinaryEqual(file1, file2));
    }

    [Test]
    public void DifferentFilesReturnFalse()
    {
        string file1 = Path.Combine(_tempDir, "file1.bin");
        string file2 = Path.Combine(_tempDir, "file2.bin");

        File.WriteAllBytes(file1, [1, 2, 3, 4]);
        File.WriteAllBytes(file2, [1, 2, 9, 4]);

        Assert.IsFalse(FileAssert.AreBinaryEqual(file1, file2));
    }

    [Test]
    public void EmptyFilesReturnTrue()
    {
        string file1 = Path.Combine(_tempDir, "empty1.bin");
        string file2 = Path.Combine(_tempDir, "empty2.bin");

        File.WriteAllBytes(file1, []);
        File.WriteAllBytes(file2, []);

        Assert.IsTrue(FileAssert.AreBinaryEqual(file1, file2));
    }
}
