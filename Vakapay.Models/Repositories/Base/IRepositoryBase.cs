using System.Collections.Generic;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBase<TModel>
    {
        ReturnObject Update(TModel objectUpdate);
        ReturnObject Delete(string id);
        ReturnObject Insert(TModel objectInsert);
        TModel FindById(string id);
        List<TModel> FindBySql(string sqlString);

        ReturnObject ExecuteSql(string sqlString, object transaction = null);

        int ExcuteCount(string sql);
        //ReturnObject SafeUpdate(TModel objectUpdate);
    }
}