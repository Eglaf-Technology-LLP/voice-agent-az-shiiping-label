using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class BCProductResponse
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string number { get; set; }
        public string id { get; set; }
        public string displayName { get; set; }
        public string displayName2 { get; set; }
        public string type { get; set; }
        public string itemCategoryId { get; set; }
        public string itemCategoryCode { get; set; }
        public bool blocked { get; set; }
        public decimal inventory { get; set; }
        public decimal unitPrice { get; set; }
        public bool priceIncludesTax { get; set; }
        public decimal unitCost { get; set; }
        public string taxGroupId { get; set; }
        public string taxGroupCode { get; set; }
        public string baseUnitOfMeasureId { get; set; }
        public string baseUnitOfMeasureCode { get; set; }
        public string generalProductPostingGroupId { get; set; }
        public string generalProductPostingGroupCode { get; set; }
        public string inventoryPostingGroupId { get; set; }
        public string inventoryPostingGroupCode { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public bool allowInvoiceDisc { get; set; }
        public string priceProfitCalculation { get; set; }
        public decimal profitPercent { get; set; }
        public string costingMethod { get; set; }
        public decimal standardCost { get; set; }
        public decimal lastDirectCost { get; set; }
        public decimal indirectCostPercent { get; set; }
        public bool costisAdjusted { get; set; }
        public bool allowOnlineAdjustment { get; set; }
        public string vendorNo { get; set; }
        public string vendorItemNo { get; set; }
        public string leadTimeCalculation { get; set; }
        public decimal reorderPoint { get; set; }
        public decimal maximumInventory { get; set; }
        public decimal reorderQuantity { get; set; }
        public string alternativeItemNo { get; set; }
        public decimal unitListPrice { get; set; }
        public decimal dutyDuePercent { get; set; }
        public string dutyCode { get; set; }
        public decimal grossWeight { get; set; }
        public decimal netWeight { get; set; }
        public decimal unitsPerParcel { get; set; }
        public decimal unitVolume { get; set; }
        public string durability { get; set; }
        public string freightType { get; set; }
        public string tariffNo { get; set; }
        public decimal dutyUnitConversion { get; set; }
        public string countryRegionPurchasedCode { get; set; }
        public decimal budgetQuantity { get; set; }
        public decimal budgetedAmount { get; set; }
        public decimal budgetProfit { get; set; }
        public bool comment { get; set; }
        public bool costIsPostedToGL { get; set; }
        public string blockReason { get; set; }
        public string lastDateModified { get; set; }
        public string lastTimeModified { get; set; }
        public string dateFilter { get; set; }
        public string globalDimension1Filter { get; set; }
        public string globalDimension1Code { get; set; }
        public string globalDimension2Filter { get; set; }
        public string globalDimension2Code { get; set; }
        public string locationFilter { get; set; }
        public decimal netInvoicedQty { get; set; }
        public decimal netChange { get; set; }
        public decimal purchasesQty { get; set; }
        public decimal salesQty { get; set; }
        public decimal positiveAdjmtQty { get; set; }
        public decimal negativeAdjmtQty { get; set; }
        public decimal purchasesLCY { get; set; }
        public decimal salesLCY { get; set; }
        public decimal positiveAdjmtLCY { get; set; }
        public decimal negativeAdjmtLCY { get; set; }
        public decimal cOGSLCY { get; set; }
        public decimal qtyonPurchOrder { get; set; }
        public decimal qtyonSalesOrder { get; set; }
        public bool priceIncludesVAT { get; set; }
        public string dropShipmentFilter { get; set; }
        public string vATBusPostingGrPrice { get; set; }
        public string genProdPostingGroup { get; set; }
        public string picture { get; set; }
        public decimal transferredQty { get; set; }
        public decimal transferredLCY { get; set; }
        public string countryRegionofOriginCode { get; set; }
        public bool automaticExtTexts { get; set; }
        public string noSeries { get; set; }
        public string vATProdPostingGroup { get; set; }
        public string reserve { get; set; }
        public decimal reservedQtyonInventory { get; set; }
        public decimal reservedQtyonPurchOrders { get; set; }
        public decimal reservedQtyonSalesOrders { get; set; }
        public decimal resQtyonOutboundTransfer { get; set; }
        public decimal resQtyonInboundTransfer { get; set; }
        public decimal resQtyonSalesReturns { get; set; }
        public decimal resQtyonPurchReturns { get; set; }
        public string stockoutWarning { get; set; }
        public string preventNegativeInventory { get; set; }
        public string variantMandatoryifExists { get; set; }
        public decimal costofOpenProductionOrders { get; set; }
        public string applicationWkshUserID { get; set; }
        public bool coupledtoDataverse { get; set; }
        public string assemblyPolicy { get; set; }
        public decimal resQtyonAssemblyOrder { get; set; }
        public decimal resQtyonAsmComp { get; set; }
        public decimal qtyonAssemblyOrder { get; set; }
        public decimal qtyonAsmComponent { get; set; }
        public decimal qtyonJobOrder { get; set; }
        public decimal resQtyonJobOrder { get; set; }
        public string defaultDeferralTemplateCode { get; set; }
        public decimal lowLevelCode { get; set; }
        public string serialNos { get; set; }
        public string lastUnitCostCalcDate { get; set; }
        public decimal rolledupMaterialCost { get; set; }
        public decimal rolledupCapacityCost { get; set; }
        public bool inventoryValueZero { get; set; }
        public decimal discreteOrderQuantity { get; set; }
        public decimal minimumOrderQuantity { get; set; }
        public decimal maximumOrderQuantity { get; set; }
        public decimal safetyStockQuantity { get; set; }
        public decimal orderMultiple { get; set; }
        public string safetyLeadTime { get; set; }
        public string replenishmentSystem { get; set; }
        public decimal scheduledReceiptQty { get; set; }
        public string binFilter { get; set; }
        public string variantFilter { get; set; }
        public string salesUnitofMeasure { get; set; }
        public string purchUnitofMeasure { get; set; }
        public string unitofMeasureFilter { get; set; }
        public string timeBucket { get; set; }
        public decimal resQtyonReqLine { get; set; }
        public string reorderingPolicy { get; set; }
        public bool includeInventory { get; set; }
        public string manufacturingPolicy { get; set; }
        public string reschedulingPeriod { get; set; }
        public string lotAccumulationPeriod { get; set; }
        public string dampenerPeriod { get; set; }
        public string variationID { get; set; }
        public string conditionID { get; set; }
        public string gTIN_13 { get; set; }
        public string brandName { get; set; }
        public string groups { get; set; }
        public string medicationForm { get; set; }
        public string? medication_Quantity { get; set; }
        public string treatmentType { get; set; }
        public string strength { get; set; }
        public string stregthUnitofMeasurement { get; set; }
        public string? dosageLabel { get; set; }
        public string? taxRate { get; set; }
    }
}
