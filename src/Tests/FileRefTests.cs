namespace Tests
{
    using System;
    using System.IO;

    using ObscureWare.TestTools.Files;

    using Shouldly;

    using Xunit;

    public class FileRefTests
    {
        [Fact]
        public void simple_file_creeation_builds_proper_object()
        {
            const string FILE_NAME = @"test.txt";

            var fileRef = FileRef.Create(FILE_NAME);

            fileRef.ShouldNotBeNull();
            fileRef.Name.ShouldBe(FILE_NAME);
        }

        [Fact]
        public void file_specified_by_virtual_path_shall_be_properly_rooted()
        {
            const string EXT = @".txt";
            const string FILE_NAME = @"test";
            const string FULL_FILE_NAME = FILE_NAME + EXT;
            string currDir = Environment.CurrentDirectory;
            string expectedPath = Path.Combine(currDir, FULL_FILE_NAME);

            var fileRef = FileRef.CreateVirtual(FULL_FILE_NAME);

            fileRef.ShouldNotBeNull();
            fileRef.Name.ShouldBe(FULL_FILE_NAME);
            fileRef.SimpleName.ShouldBe(FILE_NAME);
            fileRef.Extension.ShouldBe(EXT);
            fileRef.FullPath.ShouldBe(expectedPath);
        }

        [Theory]
        //[InlineData(@"C:\tests\tts*s.s")]
        //[InlineData(@"C:\tests\tts?s.s")]
        //[InlineData(@"C:\tests\tts:s.s")]
        [InlineData(@"C:\tests\tts|s.s")]
        [InlineData(@"C:\te|sts\ttss.s")]
        [InlineData(@"C:\tests\tts<s.s")]
        [InlineData(@"C:\tests\tts>s.s")] // TODO: add more cases?
        public void invalid_characters_in_file_name_shall_be_validated_when_asked_on_absolute_refs(string fName)
        {
            FileRef reff;
            Should.Throw<ArgumentException>(() => reff = FileRef.CreateAbsolute(fName));

            //fileRef.ShouldNotBeNull();
            //fileRef.FileNameIsValid.ShouldBeFalse();
        }

        [Theory]
        [InlineData(@"C:\tests\tts>s.s")]
        [InlineData(@"C:\tests\tts|s.s")]
        public void invalid_characters_in_file_name_shall_throw_exceptions_during_creation_of_virtual_refs(string fName)
        {
            FileRef reff;
            Should.Throw<ArgumentException>(() => reff = FileRef.CreateVirtual(fName));
        }

        [Theory]
        [InlineData(@"C:\tests\tts|s.s")]
        [InlineData(@"C:\tests\tts<s.s")]
        public void invalid_characters_in_file_name_shall_throw_exceptions_during_creation_of_automated_refs(string fName)
        {
            FileRef reff;
            Should.Throw<ArgumentException>(() => reff = FileRef.Create(fName));
        }

        [Theory]
        [InlineData(@"C:\tests\tts.s")]
        [InlineData(@"\tests\ttss.s")]
        public void absolute_paths_shall_throw_exceptions_during_creation_of_virtual_refs(string fName)
        {
            FileRef reff;
            Should.Throw<ArgumentException>(() => reff = FileRef.CreateVirtual(fName));
        }

        [Theory]
        [InlineData(@"C:\tests\tts*s.s")]
        [InlineData(@"C:\tests\tts?s.s")]
        [InlineData(@"C:\tests\tts:s.s")]
        [InlineData(@"C:\tests\tts|s.s")]
        [InlineData(@"C:\tests\tts<s.s")]
        [InlineData(@"C:\tests\tts>s.s")] // TODO: add more cases?
        public void invalid_characters_in_file_name_or_path_shall_be_validated_only_when_asked_on_absolute_refs(string fName)
        {
            var fileRef = FileRef.CreateAbsolute(fName);

            fileRef.ShouldNotBeNull();
            fileRef.FilePathIsValid.ShouldBeFalse();
        }
    }
}
