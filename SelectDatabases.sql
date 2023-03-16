select *  from ClientManagement.dbo.Client;
select *  from Invoicing.dbo.Client;
select *  from Notification.dbo.Client;
select *  from PortfolioManagement.dbo.Client;

select *  from StockManagement.dbo.Stock;
select *  from PortfolioManagement.dbo.Stock;

select *  from Invoicing.dbo.Invoice;

select *  from Invoicing.dbo.Trading;
--update Invoicing.dbo.Trading set InvoiceSent = 0
select *  from Notification.dbo.Trading;
select *  from PortfolioManagement.dbo.Trading;

select *  from PortfolioManagementEventStore.dbo.PortfolioPlanningEvent;
select *  from PortfolioManagementEventStore.dbo.PortfolioPlanning;
