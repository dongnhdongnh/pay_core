using System;
using System.Collections.Generic;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBase<TModel>
    {
        ReturnObject Update(TModel objectUpdate);
        ReturnObject Delete(string Id);
        ReturnObject Insert(TModel objectInsert);
        TModel FindById(string Id);
        List<TModel> FindBySql(string sqlString);
        //ReturnObject SafeUpdate(TModel objectUpdate);

    }
}