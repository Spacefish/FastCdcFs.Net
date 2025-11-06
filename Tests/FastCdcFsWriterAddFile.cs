using FastCdcFs.Net;

namespace Tests;

public class FastCdcFsWriterAddFile : TestBase
{

    [Theory]
    [InlineData("root")]
    [InlineData("non/root")]
    public void SameFileTwiceThrowsException(string targetPath)
    {
        var writer = new FastCdcFsWriter(FastCdcFsOptions.Default);
        writer.AddFile((byte[])[], targetPath);
        Assert.Throws<FastCdcFsFileAlreadyExistsException>(() => writer.AddFile((byte[])[], targetPath));
    }
}
