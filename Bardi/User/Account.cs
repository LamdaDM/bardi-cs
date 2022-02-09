using System;
using System.Collections.Generic;
using System.Text;

namespace Bardi.User;

public class Account
{
    public override string ToString()
    {
        var sb = new StringBuilder()
            .Append("account[0]: {\n");

        foreach (var asset in Assets) sb.Append($"asset[{asset.Idx}] : {asset.Value}\n");

        return sb.Append("}")
            .ToString();
    }

    public readonly int Idx;
    public List<Asset> Assets { get; private set; }
    public decimal TotalValue
    {
        get
        {
            var totalValue = 0m;
            foreach (var asset in Assets) totalValue += asset;
            return totalValue;
        }
    }

    public Account(int idx = 0)
    {
        Idx = idx;
        Assets = new List<Asset>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assets"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public void RegisterAssets(List<Asset> assets)
    {
        Assets = assets ?? throw new ArgumentNullException(nameof(assets));
        foreach (var asset in assets)
            if (!asset.IsOwnedByAccount())
                throw new Exception($"Asset {asset.Idx} cannot be added to Account {Idx} due to owner discrepancy.");
    }
}