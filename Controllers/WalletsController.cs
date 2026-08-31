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
    [HttpPost("{id}/credit")]
    public ActionResult<List<Wallet>> CreateWallet(string id)
    {
        try
        {
            var wallet = wallets.FirstOrDefault(x => x.transactionId == id);
            if (wallet != null)
            {
                wallets.Add(wallet);
                return Accepted(wallet);
            }

            Wallet newWallet = new()
            {
                transactionId = id,
            };
            object creditObj = newWallet.Credit(1000);
            wallets.Add(newWallet);

            return Created("somewhere", creditObj);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }
    // controller for testing purposes
    [HttpPut("refresh")]
    public IActionResult RefreshData()
    {
        wallets.Clear();
        return Ok(wallets);
    }

}

