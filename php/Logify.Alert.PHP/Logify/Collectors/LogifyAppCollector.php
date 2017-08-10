<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class LogifyAppCollector implements iCollector {

    private $version;
    private $name;
    private $userId;
    function __construct($appName, $appVersion, $userId) {
        $this->name = $appName;
        $this->version = $appVersion;
        $this->userId = $userId;
    }
    #region iCollector Members
    function DataName() {
        return 'logifyApp';
    }
    public function CollectData() {
        $result = array(
            'name' => $this->name,
            'version' => $this->version,
            'userId' => $this->userId,
        );
        return $result;
    }
    #endregion
}
?>