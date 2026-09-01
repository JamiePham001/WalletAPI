using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletAPI.Data;
using WalletAPI.models;

namespace WalletAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WalletsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    public WalletsController(ApplicationDbContext dbContext) => _dbContext = dbContext;

    // private static readonly List<Wallet> wallets = [];
    [HttpGet]
    public async Task<ActionResult<List<Wallet>>> GetWallets()
    {
        return Ok(await _dbContext.Wallet.ToListAsync());
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<List<Wallet>>> GetWalletById(string id)
    {
         var wallet = await _dbContext.Wallet.FindAsync(id);
        if (wallet is null)
        {
            return NotFound();
        }

        return Ok(wallet);
    }
    [HttpPost("{id}/credit")]
    public async Task<ActionResult<List<Wallet>>> CreateWallet(string id, [FromBody] RequestBody req)
    {
        try
        {
            var wallet = await _dbContext.Wallet.FindAsync(id);
            // if wallet doesnt exist, create a new wallet
            if (wallet is null)
            {
                Wallet newWallet = new(id);
                object? credittedWallet = newWallet.Credit(req.coins, req.transactionId);
                await _dbContext.Wallet.AddAsync(newWallet);
                await _dbContext.SaveChangesAsync();
                return Created("somewhere", credittedWallet);
            }

            // if wallet exists, check for idempotency
            Wallet? creditObj = wallet.Credit(req.coins, req.transactionId);
            if (creditObj is null)
            {
                return Accepted(wallet);
            }

            _dbContext.Wallet.Update(creditObj);
            await _dbContext.SaveChangesAsync();
            return Created("somewhere", creditObj);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }
    [HttpPost("{id}/debit")]
    public async Task<ActionResult<List<Wallet>>> DebitWallet(string id, [FromBody] RequestBody req)
    {
        try
        {
            // check if wallet exists 
            var wallet = await _dbContext.Wallet.FindAsync(id);
            if (wallet is null)
            {
                return NotFound("Id returned with no matches.");
            }
            Wallet? debitObj = wallet.Debit(req.coins, req.transactionId);
            if (debitObj is null)
            {
                return Accepted(wallet);
            }

            _dbContext.Wallet.Update(debitObj);
            await _dbContext.SaveChangesAsync();
            return Created("somewhere", debitObj);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }


    }
    // controller for testing purposes
    // Deletes all rows in table
    [HttpPut("refresh")]
    public async Task<IActionResult> RefreshData()
    {
        await _dbContext.Wallet.ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
        return Ok();
    }
}

public record RequestBody(string transactionId, int coins);
