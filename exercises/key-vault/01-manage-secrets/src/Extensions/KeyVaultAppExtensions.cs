using Azure.Identity;

namespace KeyVaultApp.Extensions;

public static class KeyVaultAppExtensions
{
    public static void InitializeKeyVault(this ConfigurationManager config)
    {
        Uri vaultUri = new($"https://{config["VaultName"]}.vault.azure.net/");

        config.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
    }
}