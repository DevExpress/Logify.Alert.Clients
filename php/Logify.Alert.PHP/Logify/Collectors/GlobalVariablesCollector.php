<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class GlobalVariablesCollector implements iCollector {

    private $collectors = array();
    private $permissions;
    function __construct($permissions) {
        $this->permissions = $permissions;
        $this->PlugCollector('get', $_GET);
        $this->PlugCollector('post', $_POST);
        $this->PlugCollector('cookie', $_COOKIE);
        $this->PlugCollector('files', $_FILES);
        $this->PlugCollector('environment', $_ENV);
        $this->PlugCollector('request', $_REQUEST);
        $this->PlugCollector('server', $_SERVER);
    }
    private function PlugCollector($name, $variables) {
        if (key_exists($name, $this->permissions) && $this->permissions[$name]) {
            $this->collectors[] = new VariablesCollector($name, $variables);
        }
    }
    #region iCollector Members
    function DataName() {
        return 'globals';
    }
    function CollectData() {
        $result = array();
        if (count($this->collectors) > 0) {
            foreach ($this->collectors as $collector) {
                if ($collector->HaveData()) {
                    $result[$collector->DataName()] = $collector->CollectData();
                }
            }
        }
        return $result;
    }
    #endregion
}
?>