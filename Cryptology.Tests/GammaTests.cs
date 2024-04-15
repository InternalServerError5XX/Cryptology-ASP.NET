using Cryptology.BLL;
using Cryptology.Domain.ViewModels;

namespace Cryptology.Tests;

[TestClass]
public class GammaTests
{
    private GlobalService _globalService;
    private GammaService _gammaService;
    private GammaViewModel gamma1;
    private GammaViewModel gamma2;

    [TestInitialize]
    public void Setup()
    {
        _globalService = new GlobalService();
        _gammaService = new GammaService(_globalService);

        gamma1 = new GammaViewModel()
        {
            Text = "abc"
        };
        gamma2 = new GammaViewModel()
        {
            Text = "Test"
        };
    }

    [TestMethod]
    public async Task DecryptionTest()
    {
        await _gammaService.Encrypt(gamma1);
        await _gammaService.Encrypt(gamma2);
        
        var decrypted1 = await _gammaService.Decrypt(gamma1);
        var decrypted2 = await _gammaService.Decrypt(gamma2);
        
        Assert.AreEqual(decrypted1.Data.Decrypted, gamma1.Text);
        Assert.AreEqual(decrypted2.Data.Decrypted, gamma2.Text);
    }
    
    [TestMethod]
    public async Task FrequencyTableTest()
    {
        var table1 = await _gammaService.FrequencyTable(gamma1);
        var table2 = await _gammaService.FrequencyTable(gamma2);
        
        var frequencyDictionary1 = new Dictionary<char, int>()
        {
            { 'a', 1 },
            { 'b', 1 },
            { 'c', 1 }
        };
        var frequencyDictionary2 = new Dictionary<char, int>()
        {
            { 't', 2 },
            { 'e', 1 },
            { 's', 1 }
        };

        CollectionAssert.AreEqual(table1.Data.FrequencyTable, frequencyDictionary1);
        CollectionAssert.AreEqual(table2.Data.FrequencyTable, frequencyDictionary2);
    }
    
    [TestMethod]
    public async Task SaveToFileTest()
    {
        var saved = await _gammaService.SaveToFile(gamma1);

        Assert.IsNotNull(saved);
        Assert.AreEqual(saved.Data.Text, gamma1.Text);
    }
}