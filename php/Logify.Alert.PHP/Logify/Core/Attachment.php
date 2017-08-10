<?php
namespace DevExpress\Logify\Core;

class Attachment {

    public $name;
    public $mimeType;
    public $content;
    function GetAttachmentData() {
        $result = array();
        $result['name'] = $this->name;
        $result['mimeType'] = $this->mimeType;
        $result['content'] = $this->GetEncodedContent();
        $result['compress'] = 'gzip';
        return $result;
    }
    private function GetEncodedContent() {
        $data = $this->content;
        if (is_array($this->content)) {
            $data = implode(array_map("chr", $this->content));
        }
        return base64_encode(gzencode($data, 9));
    }
}
?>