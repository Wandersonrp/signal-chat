namespace SignalChat.Api.Data;

public class ConnectionManager
{
    private readonly Dictionary<string, HashSet<string>> _connections = new();

    public int Count
    {
        get
        {
            return _connections.Count;
        }
    }

    public void Add(string connectionKey, string connectionId)
    {
        lock(_connections)
        {
            HashSet<string> connections;

            if(!_connections.TryGetValue(connectionKey, out connections))
            {
                connections = new HashSet<string>();
                _connections.Add(connectionKey, connections);
            }

            lock(connections)
            {
                connections.Add(connectionId);
            }
        }
    }

    public string GetConnection(string key)
    {
        var connection = _connections[key];

        if(connection is not null)
        {
            return connection.ToString()!;
        }

        return string.Empty;

    }

    public Dictionary<string, HashSet<string>> GetUsers()
    {
        return _connections;
    }

    public IEnumerable<string> GetConnections(string key)
    {
        HashSet<string> connections;

        if(_connections.TryGetValue(key, out connections))
        {
            return connections;
        }

        return Enumerable.Empty<string>();
    }

    public void Remove(string key, string connectionId)
    {
        lock(_connections)
        {
            HashSet<string> connections;

            if(!_connections.TryGetValue(key, out connections))
            {
                return;
            }

            lock(connections)
            {
                connections.Remove(connectionId);

                if(connections.Count == 0)
                {
                    _connections.Remove(key);
                }
            }
        }
    }
}
