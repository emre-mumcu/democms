using src.App_Data.Entities;

namespace src.App_Lib.Abstract;

public interface IActionLogService
{
	Task<int> InsertLog(ActionLog actionLog);
	Task<int> InsertLog(ExceptionLog exceptionLog);
}