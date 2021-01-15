# UnitOfWork

UnitOfWork is a Dot Net Core Library to implemented for unit of work patterns, Query Handler Patter, repository and direct MS SQL queries using dapper.

### Note**
Keeping source code private for now. If you you all think its something which you want to use then i will make it public.

## Installation

Use [Nuget](https://www.nuget.org/packages/UnitOfWork.Service) the package manager to install UnitOfWork.

```bash
PM > Install-Package HKumar.UnitOfWork
```

## Usage

### Regular usage for EFCore

```C#
using UnitOfWork.Service;

public void ConfigureServices(IServiceCollection services)
{
    var connectionString = "Your connection string";

    // TKey :- Primary column type for your identity tables, ex:- int,string,Guid. It is used for Audit purporse. In Future will create extension to configure without audit settings.
    //YourDbContext :- DbContext.
    //SqlScriptContext:- This is the default ISqlScriptContext you can override and modify according to your need.
    services.RegisterUnitOfWork<TKey, YourDbContext, SqlScriptContext>(connectionString);


    //Register Unit Of Work with Query Handler Pattern
    // TKey :- Primary column type for your identity tables, ex:- int,string,Guid. It is used for Audit purporse. 
    // In Future will create extension to configure audit settings.
    //YourDbContext :- DbContext.
    //SqlScriptContext:- This is the default ISqlScriptContext you can override and modify according to your need.
    //TQueryHandlerAssemblyFromType :- Assembly in which all your IQueryHandler Defined so that it can add all the 
    //Handlers into DI pipeline automatically
    services
        .RegisterUnitOfWork<TKey, YourDbContext, SqlScriptContext, TQueryHandlerAssemblyFromType>(connectionString);
}

public class TestController: Controller{
    
    private readonly IUnitOfWork _unitOfWork;

    public TestController(IUnitOfWork unitOfWork)
    {      
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> TestService()
    {
        var countryRepo = _unitOfWork.Repository<Country>();
        var country = countryRepo.GetFirstOrDefault(x => x.Id == 2);
        var countries = countryRepo.Where(x => x.Id == 2);

        countryRepo.Insert(new Country{ Name = "USA", Id = 2 });
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> ExecuteAsync(object parameter)
    {
        var sqlQuery = @"EXEC [TreeSetup].[usp_RemoveTree] @TreeId = @TreeId";
        var result = (await _sqlContext.ExecuteAsync(sqlQuery, parameter));
        return result;
    }
}

```

### Executing SP or Raw SQL Queries to the database.
```C#
public class TestController: Controller{
    
    private readonly IUnitOfWork _unitOfWork;

    public TestController(IUnitOfWork unitOfWork)
    {      
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> TestService()
    {
        //Executing SP or Script for which we dont require any return object
        var sqlQuery = @"EXEC [usp_RemoveItem] Id = @ItemId";
        int result = await _unitOfWork.ExecuteSqlAsync(sqlQuery, new {ItemId = 1});
        
        //Executing SP or Script for which will return a dataset of a object
        sqlQuery = @"SELECT * FROM Items";
        IEnumerable<Item> items = await _unitOfWork.ExecuteSqlAsync<Item>(sqlQuery, null);

        //Executing SP or Script for which will return multiple datasets of different objects, 
        //It will return the dataset in the same order QueryItem Passed.
        sqlQuery = @"SELECT * FROM Items WHERE ItemId = @ItemId; SELECT * FROM ItemType WHERE ItemTypeId = @ItemTypeId";
        var querymapItems = new List<QueryMapItem>()
        {
            new QueryMapItem(typeof(Item), DataRetriveTypeEnum.List, "Item"),
            new QueryMapItem(typeof(ItemType), DataRetriveTypeEnum.List, "ItemType")
        };
        IDictionary<string, object> multipleDataSet = 
            await _unitOfWork.ExecuteSqlAsync(sqlQuery, new {ItemId = 1, ItemTypeId = 1}, querymapItems);
        var menuTypes = multipleDataSet["Item"].Convert<IEnumerable<Item>>();
        var menus = multipleDataSet["ItemType"].Convert<IEnumerable<ItemType>>();
    }
}
```

### Using SQL Query Handler Pattern to execute SP or Raw SQL Queries to the database 

#### Approach 1 - Using ISqlScriptContext Interface
```C#
public class TestController: Controller{
    
    private readonly ISqlScriptContext _sqlContext;

    public TestController(ISqlScriptContext sqlContext)
    {      
        _sqlContext = sqlContext;
    }

    public async Task<IActionResult> TestService()
    {
        //Executing SP or Script for which we dont require any return object
        var sqlQuery = @"EXEC [usp_RemoveItem] Id = @ItemId";
        int result = await _sqlContext.ExecuteAsync(sqlQuery, new {ItemId = 1});
        
        //Executing SP or Script for which will return a object
        sqlQuery = @"SELECT top 1 * FROM Items";
        Item item = await _sqlContext.GetAsync<Item>(sqlQuery, null);

        //Executing SP or Script for which will return a dataset of a object
        sqlQuery = @"SELECT * FROM Items";
        IEnumerable<Item> items = await _sqlContext.SelectAsync<Item>(sqlQuery, null);

        //Executing SP or Script for which will return multiple datasets of different objects.
        //It will return the dataset in the same order QueryItem Passed
        sqlQuery = @"SELECT * FROM Items WHERE ItemId = @ItemId; SELECT * FROM ItemType WHERE ItemTypeId = @ItemTypeId";
        var querymapItems = new List<QueryMapItem>()
        {
            new QueryMapItem(typeof(Item), DataRetriveTypeEnum.List, "Item"),
            new QueryMapItem(typeof(ItemType), DataRetriveTypeEnum.List, "ItemType")
        };
        IDictionary<string, object> multipleDataSet = 
                await _sqlContext.ExecuteQueryMultipleAsync(sqlQuery, new {ItemId = 1, ItemTypeId = 1}, querymapItems);
        var items = multipleDataSet["Item"].Convert<IEnumerable<Item>>();
        var itemTypes = multipleDataSet["ItemType"].Convert<IEnumerable<ItemType>>();
    }
}
```

#### Approach 1 - Using IQueryHandler<TParam, TResult> Interface
##### This way you can keep your service clean and move all the logic to create/write SP/SQL queries in each Functional Handler
```C#
//Query Object to map the public properties with the sql query
public class ItemQuery{
    public int Id { get; set; }
    public int Name { get; set; }
}

//  Created this to separate the logic for all different type of Entities in a different file. Do not want to overload main service class.
public class ItemQueryHandler :
        IQueryHandler<ItemQuery, IEnumerable<Item>>,
        IQueryHandler<ItemQuery, Item>
        IQueryHandler<ItemQuery, int>,
        IQueryHandler<ItemQuery, IList<ItemType>>
    {
        private readonly ISqlScriptContext _sqlContext;

        public TestDefaultRawQueryHandler(ISqlScriptContext sqlContext)
        {
            _sqlContext = sqlContext;
        }

        async Task<IList<ItemType>> IQueryHandler<ItemQuery, IList<ItemType>>.ExecuteAsync(ItemQuery parameter)
        {
            var sqlQuery = @"SELECT * FROM Items WHERE @ItemId = @ItemId; SELECT * FROM ItemType WHERE @ItemTypeId = @ItemTypeId";
            var querymapItems = new List<QueryMapItem>()
            {
                new QueryMapItem(typeof(Item), DataRetriveTypeEnum.List, "Item"),
                new QueryMapItem(typeof(ItemType), DataRetriveTypeEnum.List, "ItemType")
            };
            IDictionary<string, object> multipleDataSet = await _sqlContext.ExecuteQueryMultipleAsync(sqlQuery, new {ItemId = 1, ItemTypeId = 1}, querymapItems);
            var items = multipleDataSet["Item"].Convert<IEnumerable<Item>>();
            var itemTypes = multipleDataSet["ItemType"].Convert<IEnumerable<ItemType>>();

            foreach (var itemType in itemTypes)
            {
                itemType.items.AddRange(items.Where(x => x.ItemTypeId == itemType.Id));
            }

            return itemTypes;
        }

        async Task<IEnumerable<Menu>> IQueryHandler<ItemQuery, IEnumerable<Menu>>.ExecuteAsync(ItemQuery parameter)
        {
            var sqlQuery = @"SELECT * FROM Items WHERE NAME LIKE '%@Name%'";
            var result = (await _sqlContext.SelectAsync<Menu>(sqlQuery, parameter));
            return result;
        }

        async Task<int> IQueryHandler<ItemQuery, int>.ExecuteAsync(ItemQuery parameter)
        {
            var sqlQuery = @"EXEC [usp_CreateItem] @Id = @Id, @Name = @Name";
            var result = (await _sqlContext.ExecuteAsync(sqlQuery, parameter));
            return result;
        }

        async Task<Item> IQueryHandler<ItemQuery, Item>.ExecuteAsync(ItemQuery parameter)
        {
            var sqlQuery = @"SELECT * FROM Items WHERE Id = @Id";
            var result = (await _sqlContext.GetAsync(sqlQuery, parameter));
            return result;
        }

}

public class TestController: Controller{
    
    private readonly IQueryDispatcher _queryDispatcher;

    public TestController(IQueryDispatcher queryDispatcher)
    {      
        _queryDispatcher = queryDispatcher;
    }

    public async Task<IActionResult> TestService()
    {    
        //Executing SP or Script for which we dont require any return object
        int result = await _queryDispatcher.DispatchAsync<ItemQuery, int>(new ItemQuery{ Id = 2, Name = "Item1" });
        
        //Executing SP or Script for which will return a object
        Item item = await _queryDispatcher.DispatchAsync<ItemQuery, Item>(new ItemQuery{ Id = 2 });

        //Executing SP or Script for which will return a dataset of a object
        IEnumerable<Item> items = await _queryDispatcher.DispatchAsync<ItemQuery, IEnumerable<Item>>(new ItemQuery{ Name = "Item" });

        //Executing SP or Script for which will return multiple datasets of different objects, It will return the dataset in the same order QueryItem Passed
        sqlQuery = @"SELECT * FROM Items WHERE @ItemId = @ItemId; SELECT * FROM ItemType WHERE @ItemTypeId = @ItemTypeId";
        var querymapItems = new List<QueryMapItem>()
        {
            new QueryMapItem(typeof(Item), DataRetriveTypeEnum.List, "Item"),
            new QueryMapItem(typeof(ItemType), DataRetriveTypeEnum.List, "ItemType")
        };
        IDictionary<string, object> multipleDataSet = await _sqlContext.ExecuteQueryMultipleAsync(sqlQuery, new {ItemId = 1, ItemTypeId = 1}, querymapItems);
        var menuTypes = multipleDataSet["Item"].Convert<IEnumerable<Item>>();
        var menus = multipleDataSet["ItemType"].Convert<IEnumerable<ItemType>>();
    }
}
```
### Using on QueryHandler Pattern without EntityFramework
```C#
using UnitOfWork.Service;

public void ConfigureServices(IServiceCollection services)
{
    var connectionString = "Your connection string";

    // Using Query Handler Pattern - Use IQuery Dispatcher as as above given example.
    // SqlScriptContext:- This is the default ISqlScriptContext you can override and modify according to your need.
    // TQueryHandlerAssemblyFromType :- Assembly in which all your IQueryHandler Defined so that it can add all the Handlers into DI pipeline automatically, Ex:- StartUp
    services.RegisterQueryHandlerPattern<SqlScriptContext, TQueryHandlerAssemblyFromType>(connectionString);


    //Using Direct Query in the services class without Query Handler Pattern - Use ISqlScriptContext as above given example.
    //SqlScriptContext:- This is the default ISqlScriptContext you can override and modify according to your need.
    services.AddSqlScriptContext<SqlScriptContext>(connectionString);
}
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
