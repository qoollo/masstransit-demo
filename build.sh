dotnet publish src/MassTransitAdvancedExample.sln -c Release

mkdir -p appsettings

chmod +x ./scripts/wait-for-it.sh
chmod +x ./scripts/buildservice.sh

./scripts/buildservice.sh api_service ApiService
./scripts/buildservice.sh cart_service CartService
./scripts/buildservice.sh delivery_service DeliveryService
./scripts/buildservice.sh feedback_service FeedbackService
./scripts/buildservice.sh history_service HistoryService
./scripts/buildservice.sh orchestrator_service OrderOrchestratorService
./scripts/buildservice.sh payment_service PaymentService