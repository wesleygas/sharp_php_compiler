<?php
{
    $a = 0;
    $b = 1;
    while (($a < 99999) and ($b ==1)){
        $a = $a +1;
        echo $a;
        if ($a == 5){
            $b = 0;
        }
    }
    echo $a;
}
?>