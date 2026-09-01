using Microsoft.AspNetCore.Mvc;
using WalletAPI.models;

namespace WalletAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WalletsController : ControllerBase
{
    private static readonly List<Wallet> wallets = [];
    [HttpGet]
    public ActionResult<List<Wallet>> GetWallets()
    {
        return Ok(wallets);
    }
    [HttpGet("{id}")]
    public ActionResult<List<Wallet>> GetWalletById(string id)
    {
        var wallet = wallets.FirstOrDefault(x => x.walletId == id);
        if (wallet is null)
        {
            return NotFound();
        }

        return Ok(wallet);
    }
    [HttpPost("{id}/credit")]
    public ActionResult<List<Wallet>> CreateWallet(string id, [FromBody] RequestBody req)
    {
        try
        {
            var wallet = wallets.FirstOrDefault(x => x.walletId == id);
            // if wallet doesnt exist, create a new wallet
            if (wallet is null)
            {
                Wallet newWallet = new(id);
                object? credittedWallet = newWallet.Credit(req.coins, req.transactionId);
                wallets.Add(newWallet);
                return Created("somewhere", credittedWallet);
            }

            // if wallet exists, check for idempotency
            object? creditObj = wallet.Credit(req.coins, req.transactionId);
            if (creditObj is null)
            {
                return Accepted(wallet);
            }

            return Created("somewhere", creditObj);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }
    [HttpPost("{id}/debit")]
    public ActionResult<List<Wallet>> DebitWallet(string id, [FromBody] RequestBody req)
    {
        try
        {
            // check if wallet exists 
            var wallet = wallets.FirstOrDefault(x => x.walletId == id);
            if (wallet is null)
            {
                return NotFound("Id returned with no matches.");
            }
            object? debitObj = wallet.Debit(req.coins, req.transactionId);
            if (debitObj is null)
            {
                return Accepted(wallet);
            }

            return Created("somewhere", debitObj);
        }
        catch (ArgumentException e)
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

public record RequestBody(string transactionId, int coins);
