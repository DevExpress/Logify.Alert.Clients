<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;
use DevExpress\Logify\Core\iVariables;

class VariablesCollector implements iCollector, iVariables {

    private $name;
    private $variables;
    function __construct($name, $variables) {
        $this->name = $name;
        $this->variables = $variables;
    }
    #region iCollector Members
    function DataName() {
        return $this->name;
    }
    function CollectData() {
        $result = array();
        foreach ($this->variables as $key => $value) {
            if (strpos(strtoupper($key), 'PASSWORD') === false) {
                $result[$key] = $value;
            }
        }
        return $result;
    }
    #endregion
    #region iVariables Members
    function HaveData() {
        return !empty($this->variables);
    }
    #endregion
}
?>