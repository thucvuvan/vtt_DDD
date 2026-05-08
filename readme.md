----DATABASE--

# CREATE TABLE IF NOT EXISTS pre_event.`event_item` (
#   `id`                    BIGINT(20)   NOT NULL AUTO_INCREMENT COMMENT 'Event item ID',
#   `ev_it_title`           VARCHAR(50)  NOT NULL COMMENT 'Event title',
#   `ev_it_subtitle`        VARCHAR(50)  NULL COMMENT 'Event item subtitle',
#   `ev_it_description`     TEXT         COMMENT 'Detailed description in rich text',
#   `ev_it_initial_stock`   INT(11)      NOT NULL DEFAULT '0' COMMENT 'Initial stock quantity',
#   `ev_it_available_stock` INT(11)      NOT NULL DEFAULT '0' COMMENT 'Current available stock',
#   `ev_it_is_stock_prepared` INT(11)    NOT NULL DEFAULT '0' COMMENT 'Has stock been pre-warmed',
#   `ev_it_original_price`  BIGINT(20)   NOT NULL COMMENT 'Original price of the event item',
#   `ev_it_flash_price`     BIGINT(20)   NOT NULL COMMENT 'Flash sale price',
#   `ev_it_start_time`      DATETIME     NOT NULL COMMENT 'Flash sale start time',
#   `ev_it_end_time`        DATETIME     NOT NULL COMMENT 'Flash sale end time',
#   `ev_it_rules`           TEXT         COMMENT 'Sale rules in JSON format',
#   `ev_it_status`          INT(11)      NOT NULL DEFAULT '0' COMMENT 'Item status (e.g., active)',
#   `ev_it_activity_id`     BIGINT(20)   NOT NULL COMMENT 'Associated activity ID',
#   `ev_it_updated_at`      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
#   `ev_it_created_at`      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
#   PRIMARY KEY (`id`),
#   KEY `idx_ev_it_activity_id` (`ev_it_activity_id`),
#   KEY `idx_ev_it_status` (`ev_it_status`)
# ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Event item details table';


 INSERT INTO pre_event.event_item
 (id, ev_it_title, ev_it_subtitle, ev_it_description, ev_it_initial_stock, ev_it_available_stock, ev_it_is_stock_prepared, ev_it_original_price, ev_it_flash_price, ev_it_start_time, ev_it_end_time, ev_it_rules, ev_it_status, ev_it_activity_id, ev_it_updated_at, ev_it_created_at)
 VALUES(1, 'Vé 1', 'Vé 1', 'Vé1', 100, 100, 10, 10000, 9000, '2026-05-05 00:00:00', '2026-05-05 00:00:00', ' ', 0, 1, '2026-05-08 10:47:31', '2026-05-08 10:47:31');
 
 ---------