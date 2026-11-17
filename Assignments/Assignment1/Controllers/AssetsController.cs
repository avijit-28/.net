using InventoryApi.Data;
using InventoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/assets")]
[Authorize]
public sealed class AssetsController(InventoryRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Asset>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await repository.GetAllAssetsAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Asset>> GetById(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(new { message = "Asset ID must be positive." });
        var asset = await repository.GetAssetByIdAsync(id, cancellationToken);
        return asset is null ? NotFound() : Ok(asset);
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> Create(AssetInput input, CancellationToken cancellationToken)
    {
        var id = await repository.InsertAssetAsync(input, cancellationToken);
        var created = new Asset(id, input.AssetName, input.Category, input.PurchaseDate!.Value.Date, input.Status, input.Price);
        return CreatedAtAction(nameof(GetById), new { id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AssetInput input, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(new { message = "Asset ID must be positive." });
        return await repository.UpdateAssetAsync(id, input, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) return BadRequest(new { message = "Asset ID must be positive." });
        return await repository.DeactivateAssetAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
