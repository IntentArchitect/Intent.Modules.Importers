using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RelationalDbSchemaImporter.CLI.Providers.Core.Services;

namespace Intent.RelationalDbSchemaImporter.CLI.Providers.PostgreSQL;

internal class PostgreSQLSystemObjectFilter : DefaultSystemObjectFilter
{
    private static readonly HashSet<string> PostgresSystemSchemas = new(StringComparer.OrdinalIgnoreCase)
    {
        "information_schema", "pg_catalog", "pg_toast", "pg_temp", "pg_toast_temp",
        "sys"
    };
    private static readonly HashSet<string> PostgresSystemTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "pg_statistic", "pg_type", "pg_class", "pg_namespace", "pg_attribute",
        "pg_proc", "pg_index", "pg_constraint", "pg_description", "pg_tablespace",
        "pg_trigger", "pg_event_trigger", "pg_foreign_data_wrapper", "pg_foreign_server",
        "pg_user_mapping", "pg_shdepend", "pg_shdescription"
    };
    private static readonly HashSet<string> PostgresSystemFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        "armor", "crypt", "encrypt", "encrypt_iv", "decrypt", "decrypt_iv", "dearmor", "digest", 
        "gen_random_bytes", "gen_random_uuid", "gen_salt", "hmac", "pgp_armor_headers", "pgp_key_id", 
        "pgp_pub_decrypt", "pgp_pub_encrypt", "pgp_pub_decrypt_bytea", "pgp_pub_encrypt_bytea", 
        "pgp_sym_decrypt", "pgp_sym_encrypt", "pgp_sym_decrypt_bytea", "pgp_sym_encrypt_bytea", 
        "uuid_generate_v1", "uuid_generate_v1mc", "uuid_generate_v3", "uuid_generate_v4", 
        "uuid_generate_v5", "uuid_nil", "uuid_ns_dns", "uuid_ns_oid", "uuid_ns_url", "uuid_ns_x500", 
        "pg_sleep"
    };
    
    public override bool IsSystemObject(string? schema, string? name)
    {
        if (string.IsNullOrEmpty(name) || base.IsSystemObject(schema, name))
        {
            return true;
        }
        return (schema is not null && PostgresSystemSchemas.Contains(schema)) 
               || PostgresSystemTables.Contains(name)
               || PostgresSystemFunctions.Contains(name);
    }
} 