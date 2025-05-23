using Microsoft.AspNetCore.Mvc;
using Customer.API.Models;
using Customer.API.Services;
using System.ComponentModel;

namespace Customer.API.Controllers;

/// <summary>
/// Manages customer information and operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomerController : ControllerBase
{
    private readonly CustomerRepository _repository;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(CustomerRepository repository, ILogger<CustomerController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all customers
    /// </summary>
    /// <returns>A list of all customers</returns>
    /// <response code="200">Returns the list of customers</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Models.Customer>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_repository.GetAll());
    }

    /// <summary>
    /// Retrieves a specific customer by ID
    /// </summary>
    /// <param name="id">The ID of the customer to retrieve</param>
    /// <returns>The requested customer</returns>
    /// <response code="200">Returns the requested customer</response>
    /// <response code="404">If the customer is not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Models.Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var customer = _repository.GetById(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Searches for customers by company name
    /// </summary>
    /// <param name="companyName">The company name to search for</param>
    /// <returns>A list of matching customers</returns>
    /// <response code="200">Returns the list of matching customers</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Models.Customer>), StatusCodes.Status200OK)]
    public IActionResult Search([FromQuery] string companyName)
    {
        return Ok(_repository.SearchByName(companyName));
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    /// <param name="customer">The customer information to create</param>
    /// <returns>The created customer</returns>
    /// <response code="201">Returns the newly created customer</response>
    /// <response code="400">If the customer data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(Models.Customer), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create(Models.Customer customer)
    {
        _repository.Add(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    /// <param name="id">The ID of the customer to update</param>
    /// <param name="customer">The updated customer information</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the customer was successfully updated</response>
    /// <response code="400">If the ID doesn't match the customer's ID</response>
    /// <response code="404">If the customer is not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, Models.Customer customer)
    {
        if (id != customer.Id) return BadRequest();

        var success = _repository.Update(customer);
        if (!success) return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific customer
    /// </summary>
    /// <param name="id">The ID of the customer to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the customer was successfully deleted</response>
    /// <response code="404">If the customer is not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var success = _repository.Delete(id);
        if (!success) return NotFound();

        return NoContent();
    }
}