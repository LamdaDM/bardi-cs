using System.Collections.Generic;
using Bardi.User;
using NUnit.Framework;

namespace BardiTest;

[TestFixture]
public class UserDataTest
{
    private Account _account;

    [SetUp]
    public void Setup()
    {
        _account = new Account();
        _account.RegisterAssets(new List<Asset>
        {
            new(100, _account, 0),
            new(89999.9901m, _account, 1),
        });
    }

    [Test]
    public void AccountPrint()
    {
        var printedAccount = _account.ToString();

        var expectedPrint = "account[0]: {\nasset[0] : 100\nasset[1] : 89999.9901\n}";
        
        Assert.AreEqual(expectedPrint, printedAccount);
    }
}