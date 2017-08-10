<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class CustomDataCollector implements iCollector {

    public $customData;
    function __construct($customData) {
        $this->customData = $customData;
    }
    #region iCollector Members
    function DataName() {
        return 'customData';
    }
    public function CollectData() {
        return $this->customData;
    }
    #endregion
}
?>