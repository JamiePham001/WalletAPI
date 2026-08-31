using Microsoft.AspNetCore.Mvc;
using WalletAPI.models;

namespace WalletAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WalletsController : ControllerBase
{
    private static readonly List<Wallet> wallets =
    [
        new() {
            transactionId = "tx101",
            coins = 100,
        },
        new() {
            transactionId = "tx103",
            coins = 0,
        }
    ];
    [HttpGet]
    public ActionResult<List<Wallet>> GetWallets()
    {
        return Ok(wallets);
    }
    [HttpGet("{id}")]
    public ActionResult<List<Wallet>> GetWalletById(string id)
    {
        var wallet = wallets.FirstOrDefault(x => x.transactionId == id);
        if (wallet is null)
        {
            return NotFound();
        }

        return Ok(wallet);
    }
}

