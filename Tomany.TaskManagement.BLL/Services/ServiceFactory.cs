using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement.BLL.Services;

public class ServiceFactory
{
    private static TaskManagementContext? _context;
    private static readonly object _lock = new object();

    private static TaskManagementContext GetContext()
    {
        if (_context == null)
        {
            lock (_lock)
            {
                if (_context == null)
                {
                    _context = new TaskManagementContext();
                }
            }
        }
        return _context;
    }

    public static AccountService CreateAccountService()
    {
        var context = GetContext();
        var accountRepository = new AccountRepository(context);
        return new AccountService(accountRepository);
    }

    public static LoginService CreateLoginService()
    {
        var context = GetContext();
        var accountRepository = new AccountRepository(context);
        return new LoginService(accountRepository);
    }

    public static TaskService CreateTaskService()
    {
        var context = GetContext();
        var taskRepository = new TaskRepository(context);
        var projectRepository = new ProjectRepository(context);
        return new TaskService(taskRepository, projectRepository);
    }

    public static ProjectService CreateProjectService()
    {
        var context = GetContext();
        var projectRepository = new ProjectRepository(context);
        return new ProjectService(projectRepository);
    }

    public static ProfileService CreateProfileService()
    {
        var context = GetContext();
        var accountRepository = new AccountRepository(context);
        return new ProfileService(accountRepository);
    }

    public static EmailService CreateEmailService(SmtpSettings? smtpSettings = null)
    {
        return new EmailService(smtpSettings);
    }

    public static OtpService CreateOtpService(EmailService? emailService = null)
    {
        return new OtpService(emailService);
    }

    public static void DisposeContext()
    {
        if (_context != null)
        {
            lock (_lock)
            {
                _context?.Dispose();
                _context = null;
            }
        }
    }
}

