<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class OSCollector implements iCollector {

    #region iCollector Members
    function DataName() {
        return 'os';
    }
    function CollectData() {
        $platform = php_uname('s');
        $version = php_uname('r') . '. ' . php_uname('v');
        $architecture = php_uname('m');

        return array(
            'platform' => $platform,
            'version' => $version,
            //'is64bit' => $is64bit,
            'architecture' => $architecture,
        );
    }
    #endregion
}
?>