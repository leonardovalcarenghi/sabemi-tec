# Sabemi Front-End

Aplicação web do projeto Sabemi para visualizar contratos e acompanhar/reprocessar eventos de pagamento em tempo real.

## Tecnologias

- React 19
- TypeScript
- Vite
- Axios
- SignalR (`@microsoft/signalr`)
- Bootstrap + React Bootstrap
- React Router

## Pré-requisitos

- Node.js (recomendado: versão LTS atual)
- npm

## Configuração

As variáveis de ambiente são lidas pelo Vite (`src/config/env.ts`).

Crie um arquivo `.env` na raiz do front (`sabemi-front-app`) com:

```env
VITE_APP_NAME=Sabemi Front App
VITE_API_BASE_URL=https://localhost:7004
VITE_NOTIFICATIONS_HUB_URL=https://localhost:7004/hubs/notifications
```

> Observação: o back-end está configurado para aceitar CORS em `http://localhost:5173`.

## Como rodar

No diretório `sabemi-front-app`:

1. Instalar dependências:
   - `npm install`
2. Executar em desenvolvimento:
   - `npm run dev`
3. Build de produção:
   - `npm run build`
4. Pré-visualizar build:
   - `npm run preview`

## Fluxo esperado

1. Inicie o back-end primeiro.
2. Em seguida, execute o front-end.
3. Acesse a URL exibida pelo Vite (normalmente `http://localhost:5173`).
