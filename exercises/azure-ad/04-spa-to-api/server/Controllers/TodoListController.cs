using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using TodoListAPI.Models;

namespace TodoListAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TodoListController : ControllerBase
{
    readonly TodoContext context;

    public TodoListController(TodoContext context)
    {
        this.context = context;
    }

    private bool IsAppOnlyToken() =>
        HttpContext.User.Claims.Any(c => c.Type == "idtyp")
            ? HttpContext.User.Claims.Any(c => c.Type == "idtyp" && c.Value == "app")
            : HttpContext.User.Claims.Any(c => c.Type == "roles") && HttpContext.User.Claims.Any(c => c.Type != "scp");

    [HttpGet]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Read",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Read"
    )]
    public async Task<IActionResult> GetTodoItems() =>
        IsAppOnlyToken()
            ? Ok(await context.TodoItems.ToListAsync())
            : Ok(await context.TodoItems.Where(x => x.Owner == HttpContext.User.GetObjectId()).ToListAsync());

    [HttpGet("{id}")]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Read",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Read"
    )]
    public async Task<IActionResult> GetTodoItem(int id) =>
        IsAppOnlyToken()
            ? Ok(await context.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.Owner == HttpContext.User.GetObjectId()))
            : Ok(await context.TodoItems.FindAsync(id));

    [HttpPut("{id}")]
    [RequiredScopeOrAppPermission(
        RequredScopesConfigurationKey = "AzureAD:Scopes:Write",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
    )]
    public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
    {
        if (id != todoItem.Id || !context.TodoItems.Any(x => x.Id == id))
            return NotFound();

        if ((!IsAppOnlyToken() && context.TodoItems.Any(x => x.Id == id && x.Owner == HttpContext.User.GetObjectId()))
            ||
            IsAppOnlyToken())
        {
            if (context.TodoItems.Any(x => x.Id == id && x.Owner == HttpContext.User.GetObjectId()))
            {
                context.Entry(todoItem).State = EntityState.Modified;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!context.TodoItems.Any(e => e.Id == id))
                        return NotFound()
                    else
                        throw;
                }
            }

            return NoContent();
        }
    }

    [HttpPost]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
    )]
    public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
    {
        string owner = HttpContext.User.GetObjectId();

        if (IsAppOnlyToken())
            owner = todoItem.Owner;

        todoItem.Owner = owner;
        todoItem.Status = false;

        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
    }

    [HttpDelete("{id}")]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
    )]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        TodoItem todoItem = await context.TodoItems.FindAsync(id);

        if (todoItem is null)
            return NotFound();

        if ((!IsAppOnlyToken() && context.TodoItems.Any(x => x.Id == id && x.Owner == HttpContext.User.GetObjectId()))
            ||
            IsAppOnlyToken())
        {
            context.TodoItems.Remove(todoItem);
            await context.SaveChangesAsync();
        }

        return NoContent();
    }
}