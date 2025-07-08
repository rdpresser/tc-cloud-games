FROM grafana/agent:latest

# Copia o arquivo de configuração para o path padrão
COPY agent-config.yaml /etc/agent-config.yaml

# Comando para iniciar o agente com a config montada
CMD ["agent", "run", "--config.file=/etc/agent-config.yaml"]
