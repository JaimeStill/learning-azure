REDIS_KEY=$(az redis list-keys \
    --resource-group redis-rg \
    --name jps-cache \
    --query primaryKey \
    --output tsv)

echo "$REDIS_KEY"@jps-cache.redis.cache.windows.net:6380?ssl=true