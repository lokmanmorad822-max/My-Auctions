using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            Images = p.Images
        }).ToList();

        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();

        return Ok(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Images = product.Images
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto createDto, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Category = createDto.Category,
            Images = createDto.Images
        };

        var created = await _productService.CreateAsync(product, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new ProductDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Category = created.Category,
            Images = created.Images
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, UpdateProductDto updateDto, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Category = updateDto.Category;
        product.Images = updateDto.Images;

        var updated = await _productService.UpdateAsync(product, cancellationToken);

        return Ok(new ProductDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            Category = updated.Category,
            Images = updated.Images
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();

        await _productService.DeleteAsync(product, cancellationToken);
        return NoContent();
    }
}

