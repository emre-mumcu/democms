using System;
using src.App_Data;
using src.App_Data.Entities;
using src.App_Lib.Abstract;

namespace src.App_Lib.Concrete;

public class ActionLogService(AppDbContext appDbContext) : IActionLogService
{
    public async Task<int> InsertLog(ActionLog actionLog)
    {
        appDbContext.ActionLogs.Add(actionLog);
        return await appDbContext.SaveChangesAsync();
    }

    public async Task<int> InsertLog(ExceptionLog exceptionLog)
    {
        appDbContext.ExceptionLogs.Add(exceptionLog);
        return await appDbContext.SaveChangesAsync();
    }
}
