using System;
using System.Collections.Generic;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBase<T>
    {
        ReturnObject Update(T objectUpdate);
        ReturnObject Delete(string Id);
        ReturnObject Insert(T objectInsert);
        T FindById(string Id);
        List<T> FindBySql(string sqlString);
        
    }
}