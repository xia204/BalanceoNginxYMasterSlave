**Para las masters crear el usuario replicador**

CREATE USER 'replicador'@'%' IDENTIFIED WITH mysql_native_password BY 'admin_123';

GRANT REPLICATION SLAVE ON . TO 'replicador'@'%';

FLUSH PRIVILEGES;

SHOW MASTER STATUS;

**Copiar el file que da y position**

**Para slave Cambiar los datos pra cada replica**

CHANGE REPLICATION SOURCE TO SOURCE_HOST='mysql-master2', SOURCE_PORT=3306, SOURCE_USER='replicador', SOURCE_PASSWORD='admin_123', SOURCE_LOG_FILE='binlog.000002', SOURCE_LOG_POS=8170 FOR CHANNEL 'master2';


**Revisar que la replica se realizo de forma correcta**

START REPLICA FOR CHANNEL 'master2';

SHOW REPLICA STATUS FOR CHANNEL 'master2'\G

SHOW REPLICAS;

**Buscar esos campos para verificar que esta funcionando**

| Campo                   | Significado esperado   |
| ----------------------- | ---------------------- |
| `Replica_IO_Running`    | `Yes` ✅              |
| `Replica_SQL_Running`   | `Yes` ✅              |
| `Last_IO_Error`         | (vacío o `NULL`) ✅   |
| `Last_SQL_Error`        | (vacío o `NULL`) ✅   |
| `Seconds_Behind_Source` | `0` o valor bajo 🟢   |

**Por si algo sale mal**

Para master

REVOKE REPLICATION SLAVE ON . FROM 'replicador'@'%';

DROP USER 'replicador'@'%';

SELECT user, host FROM mysql.user WHERE user = 'replicador';

Para slave

STOP REPLICA FOR CHANNEL 'master2';

RESET REPLICA FOR CHANNEL 'master2';
