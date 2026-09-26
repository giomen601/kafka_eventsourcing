using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.Common;

namespace Ticketing.Command.Infraestructure.Repositories;

public class MongoRepository<TDocument>
: IMongoRepository<TDocument> where TDocument : IDocument
{
    private readonly IMongoCollection<TDocument> collection;
    private readonly IOptions<MongoSettings> options;

    public MongoRepository
    (
        IMongoClient mongoClient,
        IOptions<MongoSettings> options
    )
    {
        this.collection = mongoClient
        .GetDatabase(options.Value.Database)
        .GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        this.options = options;
    }
    public IQueryable<TDocument> AsQueriable()
    {
        return collection.AsQueryable();
    }

    public async Task<IClientSessionHandle> BeginSessionAsync(CancellationToken cancellationToken)
    {
        var option = new ClientSessionOptions();
        option.DefaultTransactionOptions = new TransactionOptions();
        return await collection.Database.Client.StartSessionAsync(option, cancellationToken);
    }

    public void BeginTransaction(IClientSessionHandle clientSessionHandle)
    => clientSessionHandle.StartTransaction();

    public Task CommitTransactionAsync(IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken)
    => clientSessionHandle.CommitTransactionAsync(cancellationToken);

    public void DisposeSession(IClientSessionHandle clientSessionHandle)
    => clientSessionHandle.Dispose();

    public async Task InsertOneAsync(TDocument document, IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken)
    => await collection.InsertOneAsync(clientSessionHandle, document, null, cancellationToken);

    public Task RollbackTrasactionAsync(IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken)
    => clientSessionHandle.AbortTransactionAsync();

    private protected string GetCollectionName(Type documentType)
    {
        var name = documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault();
        BsonCollectionAttribute xxx = new BsonCollectionAttribute("ccc");

        if (name is not null)
        {
            return ((BsonCollectionAttribute)name).CollectionName;
        }

        throw new ArgumentException("Unknow collection");
    }

    public async Task<IEnumerable<TDocument>> FilterByAsync
    (
        Expression<Func<TDocument, bool>> expressionFilter,
        CancellationToken cancellationToken
    )
    {
        var result = await collection.FindAsync(expressionFilter, null, cancellationToken);
        var resultList = await result.ToListAsync();
        return resultList.Any() ? resultList : Enumerable.Empty<TDocument>();
    }
}